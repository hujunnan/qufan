using System;

namespace MidProject.Models
{
    public class Bill
    {
        public string id;
        public DateTime billDate { get; set; }
        public string billDetail { get; set; }
        public float money { get; set; }

        public Bill(DateTime _billDate, string _billDetail, float _money)
        {
            id = Guid.NewGuid().ToString();
            billDate = _billDate;
            billDetail = _billDetail;
            money = _money;
        }
    }
}
