// --------------------------------------------------------------------------------------------
// <copyright file="DacTaskResultComsumerService.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
// 文件功能描述：
//
// 创建标识：20141107
//
// 修改标识：
// 修改描述：
//
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------
namespace FS.SMIS_Cloud.DAC.Consumer {
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;

using FS.SMIS_Cloud.DAC.Model;

using log4net;
using Task;
using Util;

public static class DACTaskResultConsumerService {
	private static Dictionary<string, Type> Consumers = new Dictionary<string, Type>();

	public static List<DACTaskResultConsumerQueue> Queues = new List<DACTaskResultConsumerQueue>();

	private static ILog Log = LogManager.GetLogger( "ConsumerService" );

	public static void Init() {
		Consumers = new Dictionary<string, Type>();
		Queues = new List<DACTaskResultConsumerQueue>();
		if ( System.IO.File.Exists( AppDomain.CurrentDomain.BaseDirectory + " \\consumers.xml" ) ) {
			try {
				var doc = XDocument.Load( AppDomain.CurrentDomain.BaseDirectory + "\\consumers.xml" );
				var consumerNodes = doc.Root.Element( "consumers" ).Elements();
				foreach ( var node in consumerNodes ) {
					var consumerName = node.Attribute( "name" ).Value;
					var assembly = node.Attribute( "assembly" ).Value;
					var consumerType = node.Attribute( "type" ).Value;
					var type = Assembly.LoadFrom( AppDomain.CurrentDomain.BaseDirectory + "\\" + assembly ).GetType( consumerType );

					RegisterConsumer( consumerName, type );
				}

				var queuesNodes = doc.Root.Element( "queues" ).Elements( "queue" );
				int num = 0;
				foreach ( var node in queuesNodes ) {
					var consumeType = ( node.Attribute( "sync" ) == null || node.Attribute( "sync" ).Value == "true" )
					                  ? ConsumeType.Sync
					                  : ConsumeType.Async;

					var queue = new DACTaskResultConsumerQueue( consumeType );
					var queueNodes = node.Elements();
					var consumers = new List<string>();
					foreach ( var qn in queueNodes ) {
						var name = qn.Attribute( "name" ).Value;
						var consumer = GetConsumer( name );
						var sensorTypeFilter = qn.Attribute( "sensorType" );
						if ( sensorTypeFilter != null ) {
							var filterStr = sensorTypeFilter.Value.Split( ',' );
							List<SensorType> l = new List<SensorType>();
							foreach ( var s in filterStr ) {
								SensorType st;
								if ( !Enum.TryParse( s.Trim(), true, out st ) ) {
									Log.Warn( "queue " + name + " sensorType invalid." + sensorTypeFilter );
									break;
								}
								l.Add( st );
							}

							consumer.SensorTypeFilter = l.ToArray();
						}
						queue.Enqueue( consumer );
						consumers.Add( name );
					}

					Queues.Add( queue );
					Log.Debug( "queue_" + ++num + ":" + string.Join( ",", consumers ) );
				}
			} catch ( Exception e ) {
				Log.Warn( "consumer.xml parse error", e );
			}
		} else {
			Log.Warn( "consumer.xml not found" );
		}
	}

	public static void OnDacTaskResultProduced( DACTaskResult result ) {
		if ( Queues.Count == 0 ) {
			Log.Warn( "consumer not found, check DacTaskResultConsumerService has been initialized" );
			return;
		}
		Log.DebugFormat( "Ready to call {0} consumer queue/policy", Queues.Count );
		for ( int index = 0; index < Queues.Count; index++ ) {
			var queue = Queues[index];
			if ( queue.ComsumeType == ConsumeType.Async ) {
				for ( int i = 0; i < queue.Length; i++ ) {
					try {
						var consumer = queue[i];
						var resultCopy = ObjectHelper.DeepCopy( result ); // 异步队列每个消费者一个深拷贝副本
						var t = new Task( () => consumer.ProcessResult( resultCopy ) );
						t.Start();
						Log.Debug( consumer.GetType().ToString() + "start.." );
						var number = index;
						t.ContinueWith(
						task => {
							if ( task.Exception != null ) {
								Log.ErrorFormat(
								    "queue:{0}-consumer:{1} exec error",
								    number,
								    consumer.GetType() );
							}
						} );
					} catch ( Exception e ) {
						Log.Error( "consumer queue error", e );
					}
				}
			} else {
				try {
					var resultCopy = ObjectHelper.DeepCopy( result ); // 同步队列共享一个深拷贝副本
					var t = new Task(
					() => {

						for ( int i = 0; i < queue.Length; i++ ) {
							try {
								var consumer = queue[i];
                                if (consumer == null)
                                {
                                    Log.ErrorFormat("queue[{0}] is null ", i);
                                    continue;
                                }
								long started = System.DateTime.Now.Millisecond;
								Log.DebugFormat( "[Consumer] of P{0}: {1}-{2}, DTU={3}, result={4}, sensors={5}",
								                 index, i, consumer.GetType().ToString(),
								                 resultCopy.Task.DtuID ,
								                 resultCopy.IsOK,
								                 resultCopy.SensorResults.Count );
								consumer.ProcessResult( resultCopy );
								Log.DebugFormat( "[Consumer] {0} done in {1} ms.", queue[i].GetType().ToString(),
								                 System.DateTime.Now.Millisecond - started );
							} catch ( Exception e ) {
								Log.Error( string.Format( "{0} exec error", queue[i].GetType() ), e );
							}
						}
					} );
					t.Start();
                    t.ContinueWith(task =>
                    {
                        Log.Info("DNOE!");
                        if (task.Exception != null)
                        {
                            Log.ErrorFormat("[Consumer] of P [{0}] error:{1}", index, task.Exception);
                        }
                    });
				} catch ( Exception e ) {
					Log.Error( "consumer queue error", e );
				}
			}
		}
	}

	/// <summary>
	/// 添加消费者队列(各消费队列之间是并行的)
	/// </summary>
	/// <param name="queue"></param>
	public static void AddComsumerQueue( DACTaskResultConsumerQueue queue ) {
		Queues.Add( queue );
	}

	/// <summary>
	/// 注册消费者类型
	/// </summary>
	/// <param name="consumerName">消费者类型名称(不能重复)</param>
	/// <param name="consumerType">消费者类型</param>
	public static void RegisterConsumer( string consumerName, Type consumerType ) {
		if ( Consumers.ContainsKey( consumerName ) ) {
			throw new ArgumentException( "已存在的消费者名称" );
		}

		Consumers.Add( consumerName, consumerType );
	}

	/// <summary>
	/// 获取注册的消费者类型实例
	/// </summary>
	/// <param name="consumerName">消费者类型名称</param>
	/// <returns>消费者实例</returns>
	public static IDACTaskResultConsumer GetConsumer( string consumerName ) {
		Type type = Consumers[consumerName];
		return Activator.CreateInstance( type ) as IDACTaskResultConsumer;
	}

    /// <summary>
    /// 消费者插队
    /// </summary>
    /// <param name="queueindex"></param>
    /// <param name="index"></param>
    /// <param name="consumer"></param>
    public static void InsertComsumer(int queueindex, int index, IDACTaskResultConsumer consumer)
    {
        if (Queues.Count > queueindex)
            Queues[queueindex].Insert(consumer, index);
    }
}
}