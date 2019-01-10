using System;

namespace MidProject.Models
{
    /* 
     * jobname 任务名称
     * star 星标 如果是星标 则有特殊标识 小星星点亮
     * remindat 提醒时间
     * deadline时间可选 如果有deadline hasdeadline为true 并且有倒计时显示
     * completed 完成状态 如果完成则添加到已完成清单 不在界面显示
     */
    public class TodoItem
    {
        public string id;
        public string jobName { get; set; }
        public string listBelongTo { get; set; }
        public string listid { get; set; }
        public string DDLTostring { get; set; }

        public DateTime remindAt { get; set; }
        public DateTime deadline { get; set; }

        public bool star { get; set; }
        public bool hasDeadline { get; set; }
        public bool completed { get; set; }

        //创建时默认为完成 不接受参数completed 
        public TodoItem(string _id, string _jobName, string _remindAt, string _deadline, string _hasDeadline, string _complete, string _listid)
        {
            id = _id;
            jobName = _jobName;
            remindAt = Convert.ToDateTime(_remindAt);
            deadline = Convert.ToDateTime(_deadline);
            hasDeadline = _hasDeadline != "0";
            completed = _complete != "0";
            listid = _listid;

            DDLTostring = deadline.ToString();
        }

        public TodoItem(string _jobName, DateTime _remindAt, DateTime _deadline, 
                        bool _star, bool _hasDeadline, string _listBelongTo)
        {
            id = Guid.NewGuid().ToString();
            jobName = _jobName;
            listBelongTo = _listBelongTo;

            remindAt = _remindAt;
            deadline = _deadline;

            star = _star;
            hasDeadline = _hasDeadline;
            completed = false;

            DDLTostring = deadline.ToString();
        }

        public TodoItem(string _jobName, DateTime _reminAt, DateTime _deadline, bool _hasDeadline, string _listid)
        {
            id = Guid.NewGuid().ToString();
            jobName = _jobName;

            remindAt = _reminAt;
            deadline = _deadline;

            hasDeadline = _hasDeadline;
            listid = _listid;

            DDLTostring = deadline.ToString();
        }
    }
}