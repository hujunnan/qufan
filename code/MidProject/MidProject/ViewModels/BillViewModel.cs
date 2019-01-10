using System;
using System.Collections.ObjectModel;

namespace MidProject.ViewModels
{
    public class BillViewModel
    {
        private ObservableCollection<Models.Bill> allBills = new ObservableCollection<Models.Bill>();
        public ObservableCollection<Models.Bill> AllBills { get { return allBills; } }

        private Models.Bill selectedBill = default(Models.Bill);
        public Models.Bill SelectedItem { get { return selectedBill; } set { selectedBill = value; } }

        public BillViewModel() { }

        public void AddBill(string _billDetail, float _money, DateTime _billDate)
        {
            var tmp = new Models.Bill(_billDate, _billDetail, _money);
            allBills.Add(tmp);
        }

        public void RemoveBill(string _id)
        {
            for(int x = 0; x < allBills.Count; x++)
            {
                if(allBills[x].id == _id)
                {
                    selectedBill = allBills[x];
                    allBills.RemoveAt(x);
                    break;
                }
            }
            selectedBill = null;
        }

        public void UpdateBill(string _id, float _money, DateTime _billDate, string _billDetail)
        {
            for(int x = 0; x < allBills.Count; x++)
            {
                if(allBills[x].id == _id)
                {
                    selectedBill = allBills[x];
                    allBills[x].billDate = _billDate;
                    allBills[x].billDetail = _billDetail;
                    allBills[x].money = _money;
                }
            }
        }

    }
}
