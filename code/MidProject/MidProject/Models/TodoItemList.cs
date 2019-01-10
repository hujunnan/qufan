using System;
using System.Collections.ObjectModel;

namespace MidProject.Models
{
    public class TodoItemList
    {
        public string id;
        public string listName { get; set; }
        public ObservableCollection<TodoItem> items { get; set; }
        public ObservableCollection<string> itemsid { get; set; }

        public TodoItemList(string _id, string _listName, string _itemsids)
        {
            id = _id;
            listName = _listName;
            items = new ObservableCollection<TodoItem>();
            itemsid = new ObservableCollection<string>();
            string []tmp = _itemsids.Split(',');
            foreach(string t in tmp)
            {
                itemsid.Add(t);
            }
        }

        public TodoItemList(string _listName, TodoItem[] _items)
        {
            id = Guid.NewGuid().ToString();
            listName = _listName;
            items = new ObservableCollection<TodoItem>();
            itemsid = new ObservableCollection<string>();
            if (_items != null)
            {
                foreach (TodoItem item in _items)
                {
                    items.Add(item);
                }
            }
        }

        public TodoItemList(string _listName, string[] _itemsid)
        {
            id = Guid.NewGuid().ToString();
            listName = _listName;
            items = new ObservableCollection<TodoItem>();
            itemsid = new ObservableCollection<string>();
            if (_itemsid != null)
            {
                foreach (string item in _itemsid)
                {
                    itemsid.Add(item);
                }
            }
        }
    }
}
