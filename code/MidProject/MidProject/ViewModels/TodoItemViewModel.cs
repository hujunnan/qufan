using SQLitePCL;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Popups;

namespace MidProject.ViewModels
{
    public class TodoItemViewModel
    {
        private ObservableCollection<Models.TodoItem> allItems = new ObservableCollection<Models.TodoItem>();
        public ObservableCollection<Models.TodoItem> AllItems { get { return allItems; } }

        private ObservableCollection<Models.TodoItem> todayItem = new ObservableCollection<Models.TodoItem>();
        public ObservableCollection<Models.TodoItem> TodayItem { get { return todayItem; } }

        public void search(string searchdetail)
        {
            StringBuilder result = new StringBuilder();
            var db = App.myMidProject;
            using (var statement = db.Prepare("SELECT JobName, RemindAt FROM TodoItem"))
            {
                while (SQLiteResult.ROW == statement.Step())
                {
                    string description = statement[0].ToString();
                    string time = statement[1].ToString();
                    if (description.IndexOf(searchdetail, 0) != -1 || time.IndexOf(searchdetail, 0) != -1)
                    {
                        result.Append("JobName: ").Append(statement[0].ToString()).Append(" RemindAt: ").AppendLine(statement[1].ToString());
                    }
                }
            }
            if (result.Length == 0) result.AppendLine("无信息");
            var i = new MessageDialog(result.ToString()).ShowAsync();
        }

        public void searchInLocalList(string searchdetail, ObservableCollection<Models.TodoItem> items)
        {
            StringBuilder result = new StringBuilder();
            foreach(var item in items)
            {
                string description = item.jobName;
                string time = item.remindAt.ToString();
                if (description.IndexOf(searchdetail, 0) != -1 || time.IndexOf(searchdetail, 0) != -1)
                {
                    result.Append("JobName: ").Append(description).Append(" RemindAt: ").AppendLine(time);
                }
            }
            if (result.Length == 0) result.AppendLine("无信息");
            var i = new MessageDialog(result.ToString()).ShowAsync();
        }

        public void getToday()
        {
            todayItem = new ObservableCollection<Models.TodoItem>();
            foreach(Models.TodoItem item in allItems)
            {
                if(item.completed == false && (
                    (item.hasDeadline && item.deadline.Date == DateTime.Now.Date) ||
                    item.remindAt.Date == DateTime.Now.Date))
                {
                    if (! todayItem.Contains(item))
                        todayItem.Add(item);
                }
            }

            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueueForWide310x150(true);
            updater.EnableNotificationQueueForSquare150x150(true);
            updater.EnableNotificationQueueForSquare310x310(true);
            updater.EnableNotificationQueue(true);
            updater.Clear();

            //TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);

            for (int a = todayItem.Count > 5 ? todayItem.Count - 5 : 0; a < todayItem.Count; a++)
            {
                XmlDocument tileXml = new XmlDocument();
                tileXml.LoadXml(File.ReadAllText("tiles.xml"));
                //Todo: fill the template
                var TitlesAndDetails = tileXml.GetElementsByTagName("text");
                for (int i = 0; i < TitlesAndDetails.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        TitlesAndDetails[i].ChildNodes[0].NodeValue = todayItem[a].jobName;
                    }
                    else
                    {
                        TitlesAndDetails[i].ChildNodes[0].NodeValue = todayItem[a].remindAt.ToString();
                    }
                }

                TileNotification tileNotification = new TileNotification(tileXml);
                updater.Update(tileNotification);
            }
        }

        private Models.TodoItem selectedItem = default(Models.TodoItem);
        public Models.TodoItem SelectedItem { get { return selectedItem; } set { selectedItem = value; } }

        public TodoItemViewModel() { }

        public void AddTodoItem(string _jobName, DateTime _remindAt, DateTime _deadline, bool _star, bool _hasDeadline, string _listBelongTo)
        {
            var tmp = new Models.TodoItem(_jobName, _remindAt, _deadline, _star, _hasDeadline, _listBelongTo);
            allItems.Add(tmp);
        }

        //修改
        public void AddTodoItem(Models.TodoItem tmp)
        {
            var db = App.myMidProject;
            try
            {
                using (var statement = db.Prepare("INSERT INTO TodoItem (Id, JobName, RemindAt, Deadline, HasDeadline, Complete, ListId) VALUES (?, ?, ?, ?, ?, ?, ?)"))
                {
                    statement.Bind(1, tmp.id);
                    statement.Bind(2, tmp.jobName);
                    statement.Bind(3, tmp.remindAt.ToString());
                    statement.Bind(4, tmp.deadline.ToString());
                    statement.Bind(5, tmp.hasDeadline == true ? "1" : "0");
                    statement.Bind(6, tmp.completed == true ? "1" : "0");
                    statement.Bind(7, tmp.listid);
                    statement.Step();
                }
                allItems.Add(tmp);
            }
            catch (Exception ex)
            {
                var i = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        public void AddTodoItem(string _jobName, DateTime _remindAt, DateTime _deadline, bool _hasDeadline, string _listid)
        {
            var db = App.myMidProject;
            try
            {
                var tmp = new Models.TodoItem(_jobName, _remindAt, _deadline, _hasDeadline, _listid);
                using (var statement = db.Prepare("INSERT INTO TodoItem (Id, JobName, RemindAt, Deadline, HasDeadline, Complete, ListId) VALUES (?, ?, ?, ?, ?, ?, ?)"))
                {
                    statement.Bind(1, tmp.id);
                    statement.Bind(2, tmp.jobName);
                    statement.Bind(3, tmp.remindAt.ToString());
                    statement.Bind(4, tmp.deadline.ToString());
                    statement.Bind(5, tmp.hasDeadline == true ? "1" : "0");
                    statement.Bind(6, tmp.completed == true ? "1" : "0");
                    statement.Bind(7, tmp.listid);
                    statement.Step();
                }
                allItems.Add(tmp);
            }
            catch (Exception ex)
            {
                var i = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        public void RemoveTodoItem(string _id)
        {
            try
            {
                for (int x = 0; x < allItems.Count; x++)
                {
                    if (allItems[x].id == _id)
                    {
                        allItems.Remove(allItems[x]);
                        break;
                    }
                }
                var db = App.myMidProject;
                using (var statement = db.Prepare("DELETE FROM TodoItem WHERE Id = ?"))
                {
                    statement.Bind(1, _id);
                    statement.Step();
                }                
            }
            catch (Exception ex)
            {
                var i = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }
       
        public void UpadateTodoItem(string _id, string _jobName, DateTime _remindAt, DateTime _deadline, bool _hasdeadline, string _listid)
        {
            try
            {
                var db = App.myMidProject;
                using (var statement = db.Prepare("UPDATE TodoItem SET JobName = ?, RemindAt = ?, Deadline = ?, HasDeadline = ?, Complete = ?, ListId = ? WHERE Id = ?"))
                {
                    statement.Bind(1, _jobName);
                    statement.Bind(2, _remindAt.ToString());
                    statement.Bind(3, _deadline.ToString());
                    statement.Bind(4, _hasdeadline == true ? "1" : "0");
                    statement.Bind(5, selectedItem.completed == true ? "1" : "0");
                    statement.Bind(6, _listid);
                    statement.Bind(7, _id);
                    statement.Step();
                }
                for (int x = 0; x < allItems.Count; x++)
                {
                    if (allItems[x].id == _id)
                    {
                        selectedItem = allItems[x];
                        allItems[x].jobName = _jobName;
                        allItems[x].remindAt = _remindAt;
                        allItems[x].deadline = _deadline;
                        allItems[x].hasDeadline = _hasdeadline;
                        allItems[x].listid = _listid;
                        break;
                    }
                }                
            }
            catch (Exception ex)
            {
                var i = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        public void ChangeState(string _id)
        {
            for(int x = 0; x < allItems.Count; x++)
            {
                if(allItems[x].id == _id)
                {
                    allItems[x].completed = !allItems[x].completed;
                    try
                    {
                        var db = App.myMidProject;
                        using (var statement = db.Prepare("UPDATE TodoItem SET Complete = ? WHERE Id = ?"))
                        {
                            statement.Bind(1, allItems[x].completed == true ? "1" : "0");
                            statement.Bind(2, _id);
                            statement.Step();
                        }
                    }
                    catch (Exception ex)
                    {
                        var i = new MessageDialog(ex.ToString()).ShowAsync();
                    }
                }
            }
        }

        public void loadAllItems()
        {
            var db = App.myMidProject;
            using (var statement = db.Prepare("SELECT Id, JobName, RemindAt, Deadline, HasDeadline, Complete, ListId FROM TodoItem"))
            {
                while (SQLiteResult.ROW == statement.Step())
                {
                    if (statement.DataCount != 0)
                    {
                        var newitem = new Models.TodoItem((string)statement[0], (string)statement[1], (string)statement[2], (string)statement[3], (string)statement[4], (string)statement[5], (string)statement[6]);
                        allItems.Add(newitem);
                    }
                }
            }
        }
    }
}
