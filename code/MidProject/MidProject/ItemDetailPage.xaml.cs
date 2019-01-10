using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace MidProject
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ItemDetailPage : Page
    {
        public ItemDetailPage()
        {
            this.InitializeComponent();
        }
        //每个界面都有的部分
        public ViewModels.TodoItemListViewModel todoListVM = App.todoListViewModel;
        public ViewModels.TodoItemViewModel todoItemVM = App.todoItemViewModel;
        public ViewModels.MemorandumViewModel memorandunVM = App.memorandunViewModel;
        public ViewModels.PictureWallViewModel pictureVM = App.pictureWallViewModel;
        public ViewModels.BillViewModel billVM = App.billViewModel;


        private void changePane(object sender, RoutedEventArgs e)
        {
            splitView.IsPaneOpen = !splitView.IsPaneOpen;
        }

        private void chooseAList(object sender, ItemClickEventArgs e)
        {
            todoListVM.SelectedTodoItemList = (Models.TodoItemList)(e.ClickedItem);//获取点击的list
            todoItemVM.SelectedItem = null;
            Frame.Navigate(typeof(TodoInList), todoListVM);
        }

        //Icon click handle
        private void TodayIcon_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void MemorandumIcon_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MemoPage));
        }

        private void PictureIcon_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PicturePage));
        }

        private void BillIcon_Click(object sender, RoutedEventArgs e)
        {
            //弹个窗说该功能尚未实现敬请期待
            var i = new MessageDialog("奴才正在努力开发中，请小主稍候").ShowAsync();
        }

        private void ListIcon_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ListPage));

        }
                
        private void SettingIcon_Click(object sender, RoutedEventArgs e)
        {
            //弹个窗说该功能尚未实现敬请期待
            var i = new MessageDialog("奴才正在努力开发中，请小主稍候").ShowAsync();
        }

        //独有
        private void addItem(object sender, RoutedEventArgs e)
        {
            //判断是否是更新
            //判断Information，belong，time，hasDeadline，deadline内容
            //Information是否为空
            //belong是否是当前list（判断是否有选中的list）的名字
            //time是否大于当前时间
            //hasdeadline判断是否有deadline内容，如有，则下次打开如果有过期的直接删除
            bool valid = false;
            if (Infomation.Text != "" && time.Date >= DateTime.Now.Date)
            {
                if (hasDealine.IsChecked == true && dealine.Date < time.Date)
                {
                    var i = new MessageDialog("deadline要在提醒时间之后").ShowAsync();
                }
                else
                {
                    if (todoItemVM.SelectedItem != null)
                    {
                        //更新
                        todoListVM.DeleteSingleItem(todoItemVM.SelectedItem.listid,todoItemVM.SelectedItem.id);
                        todoItemVM.UpadateTodoItem(todoItemVM.SelectedItem.id, Infomation.Text, Convert.ToDateTime(time.Date.ToString()), Convert.ToDateTime(dealine.Date.ToString()), (bool)hasDealine.IsChecked, todoItemVM.SelectedItem.id);
                        todoListVM.AddSingleItem(todoItemVM.SelectedItem.listid, todoItemVM.SelectedItem);
                    }
                    else
                    {
                        //新建
                        var tmp = new Models.TodoItem(Infomation.Text, Convert.ToDateTime(time.Date.ToString()), Convert.ToDateTime(dealine.Date.ToString()), (bool)hasDealine.IsChecked, todoListVM.SelectedTodoItemList == null ? null : todoListVM.SelectedTodoItemList.id);
                        todoItemVM.AddTodoItem(tmp);
                        if (todoListVM.SelectedTodoItemList != null)
                        {
                            todoListVM.AddSingleItem(todoListVM.SelectedTodoItemList.id, tmp);
                        }
                    }
                }
                valid = true;
            }
            else
            {
                var i = new MessageDialog("消息不能为空,提醒时间需在今天或以后").ShowAsync();
                valid = false;
            }
            if (todoListVM.SelectedTodoItemList != null)
            {
                if(valid && App.isFromMain == false) Frame.Navigate(typeof(TodoInList));
                else if(valid && App.isFromMain == true) Frame.Navigate(typeof(MainPage));
            }
            else
            {
                if(valid)Frame.Navigate(typeof(TodoInList));
            }
        }

        private void clear(object sender, RoutedEventArgs e)
        {
            Infomation.Text = "";
            //belong.SelectedItem = (object)todoItemVM.SelectedItem.listBelongTo;
            time.Date = DateTime.Today;
            hasDealine.IsChecked = false;
            dealine.Date = DateTime.Today;
        }

        private void deleteItem(object sender, RoutedEventArgs e)
        {
            if(todoItemVM.SelectedItem == null)
            {
                //新建的删除
                clear(sender, e);
            }
            else
            {
                if (todoItemVM.SelectedItem.listid == null)
                {
                    //直接删除
                    todoItemVM.RemoveTodoItem(todoItemVM.SelectedItem.id);
                    Frame.Navigate(typeof(MainPage));
                }
                else
                {
                    //先在所在list里边删除然后在todoitem里边删除
                    todoListVM.DeleteSingleItem(todoItemVM.SelectedItem.listid, todoItemVM.SelectedItem.id);
                    todoItemVM.RemoveTodoItem(todoItemVM.SelectedItem.id);
                    Frame.Navigate(typeof(TodoInList));
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //载入Information，belong，time，hasDeadline，deadline内容
            if (todoItemVM.SelectedItem != null)
            {
                Infomation.Text = todoItemVM.SelectedItem.jobName;
                time.Date = todoItemVM.SelectedItem.remindAt;
                //belong没实现
                dealine.Date = todoItemVM.SelectedItem.deadline;
                hasDealine.IsChecked = todoItemVM.SelectedItem.hasDeadline;
            }            
        }
    }
}
