using System;

namespace MidProject.Models
{
    /*
     * content 备忘录内容
     * createtime创建时间
     * 不放图片了 因为我们放也就只能放一张 这样和照片墙差不多
     */
    public class Memorandum
    {
        public string id { get; set; }
        public string content { get; set; }
        public DateTime createTime { get; set; }
        public string createTimeToString { get; set; }

        public Memorandum(string _content, DateTime _createTime)
        {
            id = Guid.NewGuid().ToString();
            content = _content;
            createTime = _createTime;
            createTimeToString = _createTime.ToString();
        }

        public Memorandum(string _id, string _content, string _createTime)
        {
            id = _id;
            content = _content;
            createTimeToString = _createTime;
            createTime = Convert.ToDateTime(_createTime);
        }
    }
}
