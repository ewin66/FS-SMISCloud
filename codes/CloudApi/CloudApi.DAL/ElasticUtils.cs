using System;
using System.Collections.Generic;
using System.Linq;
using Elasticsearch.Net;
using FreeSun.FS_SMISCloud.Server.CloudApi.Entity;
using Nest;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.DAL
{
    public class ElasticUtils
    {
        public static ElasticClient GetElasticClient(String clusterName, String ip, String port)
        {
            var node = new Uri(string.Format("{0}:{1}", ip, port));

            var settings = new ConnectionSettings(
                node,
                defaultIndex: "my-application"
            );
            return new ElasticClient(settings);
        }

        public static IIndexResponse AddDataToIndex<T>(ElasticClient client, T data, string indexName, string mappingType) where T : class
        {
            try
            {
                return client.Index(data, i => i.Index(indexName).Type(mappingType));
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public static List<T> SearchData<T>(ElasticClient client, String indexName, String mappingType, int from, int size, string[] fields, string key) where T : class
        {
            var resultInfo = client.Search<T>(s => s
                .Index(indexName)
                .Type(mappingType)
                .From(from)
                .Size(size)
                .Query(q => q
                    .MultiMatch(m => m.OnFields(fields)
                        .Query(key))
                )
                );
            return resultInfo.HitsMetaData.Hits.Select(hit => hit.Source).ToList();
        }

        public static long GetCount<T>(ElasticClient client, String indexName, String mappingType, int from, int size, string[] fields, string key) where T : class
        {

            return client.Search<T>(s => s
                .Index(indexName)
                .Type(mappingType)
                .From(from)
                .Size(size)
                .SearchType(SearchType.Count)
                .Query(q => q
                    .MultiMatch(m => m.OnFields(fields)
                        .Query(key)))).Total;
        }

        public static long GetCount<T>(ElasticClient client, String indexName, String mappingType, string key)
            where T : class
        {
            QueryContainer query = new MatchAllQuery() && new TermQuery()
            {
                Value = key
            };

            var searchRequest = new SearchRequest
            {
                Indices = new IndexNameMarker[] { indexName },
                Types = new TypeNameMarker[] { mappingType },
                Query = query,
                SearchType = SearchType.Count
            };
            return client.Search<T>(searchRequest).Total;

            //return client.Count(s => s
            //    .Index(indexName)
            //    .Type(mappingType)
            //    .Query(q => q.Match(
            //        m => m.Query(key)))).Count;
        }

        public static StructFilesData SearchStructData(ElasticClient client, String indexName, String mappingType,
            String fieldName, String key)
        {
            String rangeYear = DateTime.Now.Year + "";
            String day = (DateTime.Now.Day - 1) + "";
            String week = (DateTime.Now.DayOfWeek - 1) + "";
            String month = DateTime.Now.Month + "";
            String year = (DateTime.Now.Year - 1) + "";
            var structFilesCls = new StructFilesData();
            var daysearchResult = client.Search<SearchReturnData>(s => s
                .Index(indexName)
                .Type(mappingType)
                .Query(q => q.MatchAll()
                    && Query<object>.MatchPhrase(mp => mp.OnField("structName").Query(key))
                    && Query<object>.MatchPhrase(mp => mp.OnField("timeRange").Query(TimeRange.天.ToString()))
                    && Query<object>.MatchPhrase(mp => mp.OnField("rangeYear").Query(rangeYear))
                    && Query<object>.MatchPhrase(mp => mp.OnField("rangeValue").Query(day))
                    //.Bool(m => m.Must(mq=>mq.MatchAll(),Query<object>.MatchPhrase(mp => mp.OnField("structName").Query(key)))
                    //    .Must(Query<object>.MatchPhrase(mp => mp.OnField("timeRange").Query(TimeRange.天.ToString())))
                    //    .Must(Query<object>.MatchPhrase(mp => mp.OnField("rangeYear").Query(rangeYear)))
                    //    .Must(Query<object>.MatchPhrase(mp => mp.OnField("rangeValue").Query(day)))
                    //)
                )
                .SearchType(SearchType.DfsQueryAndFetch)
                );
            
            var dayReturnInfoClses = new SearchReturnData[daysearchResult.HitsMetaData.Hits.Count];
            for (var i = 0; i < daysearchResult.HitsMetaData.Hits.Count; i++)
            {
                dayReturnInfoClses[i] = daysearchResult.HitsMetaData.Hits[i].Source;
            }
            structFilesCls.DayFiles = dayReturnInfoClses;

            var weeksearchResult = client.Search<SearchReturnData>(s => s
                .Index(indexName)
                .Type(mappingType)
                .Query(q => q.MatchAll()
                    && Query<object>.MatchPhrase(mp => mp.OnField("structName").Query(key))
                    && Query<object>.MatchPhrase(mp => mp.OnField("timeRange").Query(TimeRange.周.ToString()))
                    && Query<object>.MatchPhrase(mp => mp.OnField("rangeYear").Query(rangeYear))
                    && Query<object>.MatchPhrase(mp => mp.OnField("rangeValue").Query(week))
                    //.Bool(m => m.Must(Query<object>.Term("structName", key))
                    //    .Must(Query<object>.Term("timeRange", TimeRange.周.ToString()))
                    //    .Must(Query<object>.Term("rangeYear", rangeYear))
                    //    .Must(Query<object>.Term("rangeValue", week))
                    //)
                )
                .SearchType(SearchType.DfsQueryAndFetch)
                );
            var weekReturnInfoClses = new SearchReturnData[weeksearchResult.HitsMetaData.Hits.Count];
            for (var i = 0; i < weeksearchResult.HitsMetaData.Hits.Count; i++)
            {
                weekReturnInfoClses[i] = weeksearchResult.HitsMetaData.Hits[i].Source;
            }
            structFilesCls.WeekFiles = weekReturnInfoClses;

            var monthsearchResult = client.Search<SearchReturnData>(s => s
                .Index(indexName)
                .Type(mappingType)
                .Query(q => q.MatchAll()
                    && Query<object>.MatchPhrase(mp => mp.OnField("structName").Query(key))
                    && Query<object>.MatchPhrase(mp => mp.OnField("timeRange").Query(TimeRange.月.ToString()))
                    && Query<object>.MatchPhrase(mp => mp.OnField("rangeYear").Query(rangeYear))
                    && Query<object>.MatchPhrase(mp => mp.OnField("rangeValue").Query(month))
                    //.Bool(m => m.Must(Query<object>.Term("structName", key))
                    //    .Must(Query<object>.Term("timeRange", TimeRange.月.ToString()))
                    //    .Must(Query<object>.Term("rangeYear", rangeYear))
                    //    .Must(Query<object>.Term("rangeValue", month))
                    //)
                )
                .SearchType(SearchType.DfsQueryAndFetch)
                );
            var monthReturnInfoClses = new SearchReturnData[monthsearchResult.HitsMetaData.Hits.Count];
            for (var i = 0; i < monthsearchResult.HitsMetaData.Hits.Count; i++)
            {
                monthReturnInfoClses[i] = monthsearchResult.HitsMetaData.Hits[i].Source;
            }
            structFilesCls.MonthFiles = monthReturnInfoClses;

            var yearsearchResult = client.Search<SearchReturnData>(s => s
                .Index(indexName)
                .Type(mappingType)
                .Query(q => q.MatchAll()
                    && Query<object>.MatchPhrase(mp => mp.OnField("structName").Query(key))
                    && Query<object>.MatchPhrase(mp => mp.OnField("timeRange").Query(TimeRange.年.ToString()))
                    && Query<object>.MatchPhrase(mp => mp.OnField("rangeYear").Query(rangeYear))
                    && Query<object>.MatchPhrase(mp => mp.OnField("rangeValue").Query(year))
                    //.Bool(m => m.Must(Query<object>.Term("structName", key))
                    //    .Must(Query<object>.Term("timeRange", TimeRange.年.ToString()))
                    //    .Must(Query<object>.Term("rangeYear", rangeYear))
                    //    .Must(Query<object>.Term("rangeValue", year))
                    //)
                )
                .SearchType(SearchType.DfsQueryAndFetch)
                );
            var yearReturnInfoClses = new SearchReturnData[yearsearchResult.HitsMetaData.Hits.Count];
            for (var i = 0; i < yearsearchResult.HitsMetaData.Hits.Count; i++)
            {
                yearReturnInfoClses[i] = yearsearchResult.HitsMetaData.Hits[i].Source;
            }
            structFilesCls.YearFiles = yearReturnInfoClses;

            return structFilesCls;
        }

        public static List<SearchReturnData> SearchMoreStructData(ElasticClient client, String indexName, String mappingType,
            String fieldName, String key)
        {
            StructFilesData structFilesData = SearchStructData(client, indexName, mappingType, fieldName,key);
            List<SearchReturnData> list = new List<SearchReturnData>();
            AddData(list, structFilesData.DayFiles, TimeRange.天.ToString());
            AddData(list, structFilesData.WeekFiles, TimeRange.周.ToString());
            AddData(list, structFilesData.MonthFiles, TimeRange.月.ToString());
            AddData(list, structFilesData.YearFiles, TimeRange.年.ToString());

            return list;

        }

        private static void AddData(List<SearchReturnData> list, SearchReturnData[] arrSearchReturnInfoClses, String str)
        {
            if (arrSearchReturnInfoClses.Length > 0)
            {
                foreach (var ri in arrSearchReturnInfoClses)
                {
                    if (ri.timeRange.Equals(TimeRange.天.ToString()))
                    {
                        ri.orders = 1;
                    }
                    else if (ri.timeRange.Equals(TimeRange.周.ToString()))
                    {
                        ri.orders = 2;
                    }
                    else if (ri.timeRange.Equals(TimeRange.月.ToString()))
                    {
                        ri.orders = 3;
                    }
                    else if (ri.timeRange.Equals(TimeRange.年.ToString()))
                    {
                        ri.orders = 4;
                    }
                    list.Add(ri);
                }
            }
            else
            {
                var info = new SearchReturnData();
                if (str.Equals(TimeRange.天.ToString()))
                {
                    // info.setTimeRange(TimeRange.DAY.toString());
                    info.orders = 1;
                }
                else if (str.Equals(TimeRange.周.ToString()))
                {
                    // info.setTimeRange(TimeRange.WEEK.toString());
                    info.orders = 2;
                }
                else if (str.Equals(TimeRange.月.ToString()))
                {
                    // info.setTimeRange(TimeRange.MONTH.toString());
                    info.orders = 3;
                }
                else if (str.Equals(TimeRange.年.ToString()))
                {
                    // info.setTimeRange(TimeRange.YEAR.toString());
                    info.orders = 4;
                }
                list.Add(info);
            }
            
        }
    }

    public enum TimeRange
    {
        天 = 0,
        周 = 1,
        月 = 2,
        年 = 3
    };

}
