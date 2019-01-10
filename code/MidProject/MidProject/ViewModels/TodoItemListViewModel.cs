using SQLitePCL;
using System;
using System.Collections.ObjectModel;
using System.Text;
using Windows.UI.Popups;

namespace MidProject.ViewModels
{
    public class TodoItemListViewModel
    {
        private ObservableCollection<Models.TodoItemList> allTodoItemLists = new ObservableCollection<Models.TodoItemList>();
        public ObservableCollection<Models.TodoItemList> AllTodoItemLists { get { return allTodoItemLists; } }

        private Models.TodoItemList selectedTodoItemList = default(Models.TodoItemList);
        public Models.TodoItemList SelectedTodoItemList { get { return selectedTodoItemList; } set { selectedTodoItemList = value; } }

        public TodoItemListViewModel() { }

        public void search(string searchdetail)
        {
            StringBuilder result = new StringBuilder();
            var db = App.myMidProject;
            using (var statement = db.Prepare("SELECT Name FROM List"))
            {
                while (SQLiteResult.ROW == statement.Step())
                {
                    string description = statement[0].ToString();
                    if (description.IndexOf(searchdetail, 0) != -1)
                    {
                        result.Append("ListName: ").Append(statement[0].ToString());
                    }
                }
            }
            if (result.Length == 0) result.AppendLine("无信息");
            var i = new MessageDialog(result.ToString()).ShowAsync();
        }

        //没有用这个函数
        public void AddList(string _listName, Models.TodoItem[] _items)
        {
            var tmp = new Models.TodoItemList(_listName, _items);
            allTodoItemLists.Add(tmp);
        }

        public void AddList(string _listName, string[] _itemsid)
        {
            var db = App.myMidProject;
            try
            {
                var tmp = new Models.TodoItemList(_listName, _itemsid);
                allTodoItemLists.Add(tmp);
                using (var statement = db.Prepare("INSERT INTO List (Id, Name, Itemsid) VALUES (?, ?, ?)"))
                {
                    statement.Bind(1, tmp.id);
                    statement.Bind(2, tmp.listName);
                    statement.Bind(3, getAllItemId(tmp.id));
                    statement.Step();
                }
            }
            catch (Exception ex)
            {
                var i = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        public void getItemsFromId(string _id)
        {
            for (int x = 0; x < allTodoItemLists.Count; x++)
            {
                if (allTodoItemLists[x].id == _id)
                {
                    for (var j = 0; j < allTodoItemLists[x].itemsid.Count; j++)
                    {
                        for (var i = 0; i < App.todoItemViewModel.AllItems.Count; i++)
                        {
                            if (allTodoItemLists[x].itemsid[j] == App.todoItemViewModel.AllItems[i].id 
                                && !allTodoItemLists[x].items.Contains(App.todoItemViewModel.AllItems[i]))
                            {
                                allTodoItemLists[x].items.Add(App.todoItemViewModel.AllItems[i]);
                                break;
                            }
                        }
                    }
                    break;
                }
            }
        }

        public void deleteAllItemsFromId(string _id)
        {
            for (int x = 0; x < allTodoItemLists.Count; x++)
            {
                if (allTodoItemLists[x].id == _id)
                {
                    for (var j = 0; j < allTodoItemLists[x].itemsid.Count; j++)
                    {
                        for (var i = 0; i < App.todoItemViewModel.AllItems.Count; i++)
                        {
                            if (allTodoItemLists[x].itemsid[j] == App.todoItemViewModel.AllItems[i].id)
                            {
                                App.todoItemViewModel.AllItems.RemoveAt(i);
                                i--;
                                break;
                            }
                        }
                    }
                    break;
                }
            }
        }
        //@id list-id
        public void AddSingleItem(string _id, Models.TodoItem _item)
        {
            var db = App.myMidProject;
            try
            {
                for (int x = 0; x < allTodoItemLists.Count; x++)
                {
                    if (allTodoItemLists[x].id == _id)
                    {
                        allTodoItemLists[x].items.Add(_item);
                        allTodoItemLists[x].itemsid.Add(_item.id);
                        break;
                    }
                }
                using (var statement = db.Prepare("UPDATE List SET Itemsid = ? WHERE Id = ?"))
                {
                    statement.Bind(1, getAllItemId(_id));
                    statement.Bind(2, _id);
                    statement.Step();
                }
            }
            catch (Exception ex)
            {
                var i = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        public void DeleteSingleItem(string _id, string _itemId)
        {
            try
            {
                for (int x = 0; x < allTodoItemLists.Count; x++)
                {
                    if (allTodoItemLists[x].id == _id)
                    {
                        foreach (Models.TodoItem item in allTodoItemLists[x].items)
                        {
                            if (item.id == _itemId)
                            {
                                allTodoItemLists[x].items.Remove(item);
                                allTodoItemLists[x].itemsid.Remove(_itemId);
                                break;
                            }
                        }
                        break;
                    }
                }
                var db = App.myMidProject;
                using (var statement = db.Prepare("UPDATE List SET Itemsid = ? WHERE Id = ?"))
                {
                    statement.Bind(1, getAllItemId(_id));
                    statement.Bind(2, _id);
                    statement.Step();
                }
            }
            catch (Exception ex)
            {
                var i = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        //直接把修改过后的item传过来
        //不能改list名字
        public void UpdateSingleItem(string _id, string _itemId, Models.TodoItem _item)
        {
            try
            {
                foreach (Models.TodoItemList list in allTodoItemLists)
                {
                    if (list.id == _id)
                    {
                        foreach (Models.TodoItem item in list.items)
                        {
                            if (item.id == _itemId)
                            {
                                list.items.Remove(item);
                                list.items.Add(_item);
                                break;
                            }
                        }
                        break;
                    }
                }
                var db = App.myMidProject;
                using (var statement = db.Prepare("UPDATE List SET Itemsid = ? WHERE Id = ?"))
                {
                    statement.Bind(1, getAllItemId(_id));
                    statement.Bind(2, _id);
                    statement.Step();
                }
            }
            catch (Exception ex)
            {
                var i = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        //整合所有itemid，方便存放到数据库
        private string getAllItemId(string _id)
        {
            string total = "";
            foreach (Models.TodoItemList list in allTodoItemLists)
            {
                if (list.id == _id)
                {
                    foreach (string item in list.itemsid)
                    {
                        total = total + item + ",";
                    }
                    break;
                }
            }
            return total;
        }

        //只修改名称 如果需要修改里面的item 直接用上面几个函数
        public void UpdateList(string _id, string _listName)
        {
            try
            {
                for (int x = 0; x < allTodoItemLists.Count; x++)
                {
                    if (allTodoItemLists[x].id == _id)
                    {
                        allTodoItemLists[x].listName = _listName;
                        break;
                    }
                }
                var db = App.myMidProject;
                using (var statement = db.Prepare("UPDATE List SET Name = ?, Itemsid = ? WHERE Id = ?"))
                {
                    statement.Bind(1, _listName);
                    statement.Bind(2, getAllItemId(_id));
                    statement.Bind(3, _id);
                    statement.Step();
                }
            }
            catch (Exception ex)
            {
                var i = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        public void DeleteList(string _id)
        {
            try
            {
                for (int x = 0; x < allTodoItemLists.Count; x++)
                {
                    if (allTodoItemLists[x].id == _id)
                    {
                        //先删除list里边所有的item
                        deleteAllItemsFromId(_id);
                        allTodoItemLists.RemoveAt(x);
                        break;
                    }
                }
                var db = App.myMidProject;
                using (var statement = db.Prepare("DELETE FROM List WHERE Id = ?"))
                {
                    statement.Bind(1, _id);
                    statement.Step();
                }
                selectedTodoItemList = null;
            }
            catch (Exception ex)
            {
                var i = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        //return the list according the id 
        public Models.TodoItemList findListFromID(string _id)
        {
            for (int x = 0; x < allTodoItemLists.Count; x++)
            {
                if (allTodoItemLists[x].id == _id)
                {
                    return allTodoItemLists[x];
                }
            }
            return null;
        }

        public void loadAllItems()
        {
            var db = App.myMidProject;
            using (var statement = db.Prepare("SELECT Id, Name, Itemsid FROM List"))
            {
                while (SQLiteResult.ROW == statement.Step())
                {
                    if (statement.DataCount != 0)
                    {
                        var newitem = new Models.TodoItemList((string)statement[0], (string)statement[1], (string)statement[2]);
                        allTodoItemLists.Add(newitem);
                        getItemsFromId(newitem.id);
                    }
                }
            }
        }

        /*public Models.TodoItemList getToday()
          {
              foreach(Models.TodoItemList list in allTodoItemLists)
              {
                  foreach(Models.TodoItem item in list.items)
                  {
                      if (((item.hasDeadline && item.deadline.Date == DateTime.Now.Date) ||
                          item.remindAt.Date == DateTime.Now.Date) &&
                          item.completed == false)
                          todayItemList.items.Add(item);
                  }
              }
              return todayItemList;
          }*/
    }
}
