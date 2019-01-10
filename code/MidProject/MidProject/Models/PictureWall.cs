using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace MidProject.Models
{
    public class PictureWall
    {
        public string id { get; set; }
        public string description { get; set; }
        public BitmapImage image { get; set; }
        public DateTime createTime { get; set; }
        public string createTimeToString { get; set; }

        public PictureWall(string _description, BitmapImage _image, DateTime _createTime)
        {
            id = Guid.NewGuid().ToString();
            description = _description;
            image = new BitmapImage();
            image = _image;
            createTime = _createTime;
            createTimeToString = _createTime.ToString();
        }
        
        public PictureWall(string _id, string _description, string _image, string _createTime)
        {
            id = _id;
            description = _description;
            image = new BitmapImage();
            image.UriSource = new Uri(_image);
            createTime = Convert.ToDateTime(_createTime);
            createTimeToString = _createTime;
        }
    }
}
