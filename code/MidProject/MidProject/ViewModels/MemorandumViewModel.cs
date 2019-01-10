using SQLitePCL;
using System;
using System.Collections.ObjectModel;
using System.Text;
using Windows.UI.Popups;

namespace MidProject.ViewModels
{
    public class MemorandumViewModel
    {
        private ObservableCollection<Models.Memorandum> allMemorandums = new ObservableCollection<Models.Memorandum>();
        public ObservableCollection<Models.Memorandum> AllMemorandums { get { return this.allMemorandums; } }

        private Models.Memorandum selectedMemorandum = default(Models.Memorandum);
        public Models.Memorandum SelectedMemorandum { get { return selectedMemorandum; } set { selectedMemorandum = value; } }

        public MemorandumViewModel() { }

        public void search(string searchdetail)
        {
            StringBuilder result = new StringBuilder();
            var db = App.myMidProject;
            using (var statement = db.Prepare("SELECT Id, Content, CreateTime FROM Mem"))
            {
                while (SQLiteResult.ROW == statement.Step())
                {
                    string description = statement[1].ToString();
                    string time = statement[2].ToString();
                    if (description.IndexOf(searchdetail, 0) != -1 || time.IndexOf(searchdetail, 0) != -1)
                    {
                        result.Append("Content: ").Append(statement[1].ToString()).Append(" CreateTime: ").AppendLine(statement[2].ToString());
                    }
                }
            }
            if (result.Length == 0) result.AppendLine("无信息");
            var i = new MessageDialog(result.ToString()).ShowAsync();
        }

        public void AddMemorandum(string _content, DateTime _createTime)
        {
            var db = App.myMidProject;
            try
            {
                var tmp = new Models.Memorandum(_content, _createTime);
                using (var statement = db.Prepare("INSERT INTO Mem (Id, Content, CreateTime) VALUES (?, ?, ?)"))
                {
                    statement.Bind(1, tmp.id);
                    statement.Bind(2, tmp.content);
                    statement.Bind(3, tmp.createTimeToString);
                    statement.Step();
                }
                allMemorandums.Add(tmp);
            }
            catch(Exception ex)
            {
                var i = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        public void RemoveMemorandum(string  _id)
        {
            try
            {
                var db = App.myMidProject;
                using (var statement = db.Prepare("DELETE FROM Mem WHERE Id = ?"))
                {
                    statement.Bind(1, _id);
                    statement.Step();
                }
                for (int x = 0; x < allMemorandums.Count; x++)
                {
                    if (allMemorandums[x].id == _id)
                    {
                        selectedMemorandum = allMemorandums[x];
                        allMemorandums.RemoveAt(x);
                        break;
                    }
                }
                selectedMemorandum = null;
            }
            catch (Exception ex)
            {
                var i = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        public void UpdateMemorandum(string _id, string _content, DateTime _createTime)
        {
            try
            {
                var db = App.myMidProject;
                using (var statement = db.Prepare("UPDATE Mem SET Content = ?, CreateTime = ? WHERE Id = ?"))
                {
                    statement.Bind(1, _content);
                    statement.Bind(2, _createTime.ToString());
                    statement.Bind(3, _id);
                    statement.Step();
                }
                for (int x = 0; x < allMemorandums.Count; x++)
                {
                    if (allMemorandums[x].id == _id)
                    {
                        selectedMemorandum = allMemorandums[x];
                        allMemorandums[x].content = _content;
                        allMemorandums[x].createTime = _createTime;
                        allMemorandums[x].createTimeToString = _createTime.ToString();
                        break;
                    }
                }
                selectedMemorandum = null;
            }
            catch (Exception ex)
            {
                var i = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        public void loadALLMemToMemorandun()
        {
            var db = App.myMidProject;
            using (var statement = db.Prepare("SELECT Id, Content, CreateTime FROM Mem"))
            {
                while (SQLiteResult.ROW == statement.Step())
                {
                    if (statement.DataCount != 0)
                    {
                        var newitem = new Models.Memorandum((string)statement[0], (string)statement[1], (string)statement[2]);
                        allMemorandums.Add(newitem);
                    }
                }
            }
        }
    }
}
