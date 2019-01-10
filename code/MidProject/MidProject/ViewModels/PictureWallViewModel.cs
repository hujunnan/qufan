using SQLitePCL;
using System;
using System.Collections.ObjectModel;
using System.Text;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;

namespace MidProject.ViewModels
{
    public class PictureWallViewModel
    {
        private ObservableCollection<Models.PictureWall> allPictureWalls = new ObservableCollection<Models.PictureWall>();
        public ObservableCollection<Models.PictureWall> AllPictureWalls { get { return allPictureWalls; } }

        private Models.PictureWall selectedPictureWall = default(Models.PictureWall);
        public Models.PictureWall SelectedPictureWall { get { return selectedPictureWall; } set { selectedPictureWall = value; } }

        public PictureWallViewModel() { }

        public void search(string searchdetail)
        {
            StringBuilder result = new StringBuilder();
            var db = App.myMidProject;
            using (var statement = db.Prepare("SELECT Description, CreateTime FROM PW"))
            {
                while (SQLiteResult.ROW == statement.Step())
                {
                    string description = statement[0].ToString();
                    string time = statement[1].ToString();
                    if (description.IndexOf(searchdetail, 0) != -1 || time.IndexOf(searchdetail, 0) != -1)
                    {
                        result.Append("Description: ").Append(statement[0].ToString()).Append(" CreateTime: ").AppendLine(statement[1].ToString());
                    }
                }
            }
            if (result.Length == 0) result.AppendLine("无信息");
            var i = new MessageDialog(result.ToString()).ShowAsync();
        }

        public void AddPictureWall(string _description, DateTime _createTime, BitmapImage _image)
        {
            var db = App.myMidProject;
            try
            {
                var tmp = new Models.PictureWall(_description, _image, _createTime);
                using (var statement = db.Prepare("INSERT INTO PW (Id, Description, CreateTime, Image) VALUES (?, ?, ?, ?)"))
                {
                    statement.Bind(1, tmp.id);
                    statement.Bind(2, tmp.description);
                    statement.Bind(3, tmp.createTimeToString);
                    statement.Bind(4, tmp.image.UriSource.ToString());
                    statement.Step();
                }
                allPictureWalls.Add(tmp);
            }
            catch (Exception ex)
            {
                var i = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        public void RemovePictureWall(string _id)
        {
            try
            {
                var db = App.myMidProject;
                using (var statement = db.Prepare("DELETE FROM PW WHERE Id = ?"))
                {
                    statement.Bind(1, _id);
                    statement.Step();
                }
                for (int x = 0; x < allPictureWalls.Count; x++)
                {
                    if (allPictureWalls[x].id == _id)
                    {
                        selectedPictureWall = allPictureWalls[x];
                        allPictureWalls.RemoveAt(x);
                        break;
                    }
                }
                selectedPictureWall = null;
            }
            catch (Exception ex)
            {
                var i = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        public void UpdatePictureWall(string _id, string _description, DateTime _createTime, BitmapImage _image)
        {
            try
            {
                var db = App.myMidProject;
                using (var statement = db.Prepare("UPDATE PW SET Description = ?, CreateTime = ?, Image = ? WHERE Id = ?"))
                {
                    statement.Bind(1, _description);
                    statement.Bind(2, _createTime.ToString());
                    statement.Bind(3, _image.UriSource.ToString());
                    statement.Bind(4, _id);
                    statement.Step();
                }
                for (int x = 0; x < allPictureWalls.Count; x++)
                {
                    if (allPictureWalls[x].id == _id)
                    {
                        selectedPictureWall = allPictureWalls[x];
                        allPictureWalls[x].createTime = _createTime;
                        allPictureWalls[x].description = _description;
                        allPictureWalls[x].image = _image;
                        break;
                    }
                }
                selectedPictureWall = null;
            }
            catch (Exception ex)
            {
                var i = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        public void loadAllItems()
        {
            var db = App.myMidProject;
            using (var statement = db.Prepare("SELECT Id, Description, CreateTime, Image FROM PW"))
            {
                while (SQLiteResult.ROW == statement.Step())
                {
                    if (statement.DataCount != 0)
                    {
                        var newitem = new Models.PictureWall((string)statement[0], (string)statement[1], (string)statement[3], (string)statement[2]);
                        allPictureWalls.Add(newitem);
                    }
                }
            }
        }
    }
}
