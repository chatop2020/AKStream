using LibCommon.Structs;
using LiteDB;

namespace AKStreamWeb.Misc
{
    public class LiteDBHelper
    {
        /// <summary>
        /// 核心的LiteDatabase数据库操作对象，这个对象私有只在类内使用。
        /// </summary>
        private LiteDatabase db;
      //  private LiteDatabase recdb;

        /// <summary>
        /// 所有的VideoChannelMediaInfo列表。
        /// </summary>
        public LiteCollection<VideoChannelMediaInfo> VideoOnlineInfo { get; set; }
      

        /// <summary>
        /// 创建一个LiteDB对象，有一个参数表明对应的文件路径。
        /// </summary>
        /// <param name="dbpath">对应数据库文件的路径，默认位置为程序目录下"VideoOnlineInfo.ldb"</param>
        public LiteDBHelper(string dbpath = "VideoOnlineInfo.ldb")
        {
            db = new LiteDatabase(dbpath);
            VideoOnlineInfo =
                (LiteCollection<VideoChannelMediaInfo>) db.GetCollection<VideoChannelMediaInfo>("VideoOnlineInfo");
           
        }
    }
}