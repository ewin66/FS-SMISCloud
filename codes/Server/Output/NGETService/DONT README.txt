***************************************************
*            ET.Service更新说明                   *
***************************************************

1.即时采集功能完善,增加采集失败的说明
2.传感器适配器热加载
	需要修改EtService.exe.config 配置文件配置C#脚本目录(绝对目录)
	<add key="ScriptPath" value="E:\SVN\FS-SMISCloud\trunk\codes\Server\Output\ETService\DAC\CxxAdapter"/>
3.传感器配置信息热加载
	热加载命令是"reload"
	当数据库中导入新的传感器配置时，"Ctrl+C"使ET进入命令模式，输入"reload"加载命令，等待加载结果，加载完成输入"exit"退出命令行模式。