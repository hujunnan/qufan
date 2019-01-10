using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class ListPage : Page
    {
        public ListPage()
        {
            this.InitializeComponent();
        }
        //共有部分
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

        private void AddIcon_Click(object sender, RoutedEventArgs e)//是添加list的+
        {
            //判断输入框内容
            if (textbox.Text == "")
            {
                var i = new MessageDialog("请输入list名字").ShowAsync();
            }
            else
            {
                string[] tmp = null;
                todoListVM.AddList(textbox.Text, tmp);
                //刷新页面
                Frame.Navigate(typeof(ListPage));
            }

        }

        private void SettingIcon_Click(object sender, RoutedEventArgs e)
        {
            //弹个窗说该功能尚未实现敬请期待
            var i = new MessageDialog("奴才正在努力开发中，请小主稍候").ShowAsync();
        }

        private void searchList(SearchBox sender, SearchBoxQuerySubmittedEventArgs e)
        {
            //应该是private async
            string searchdetail = searchbox.QueryText;
            if (searchdetail.Length == 0)
            {
                var i = new MessageDialog("搜索值为空").ShowAsync();
            }
            else
            {
                todoListVM.search(searchdetail);
            }
        }

        //独有部分
        private async void deleteList(object sender, RoutedEventArgs e)
        {
            var dialog = new MessageDialog("Sure to delete the list and all TODO items in it ?");
            dialog.Commands.Add(new UICommand("Yes", cmd => {
                dynamic ori = e.OriginalSource;
                todoListVM.SelectedTodoItemList = (Models.TodoItemList)ori.DataContext;
                todoListVM.DeleteList(todoListVM.SelectedTodoItemList.id);
            }, commandId: 0));
            dialog.Commands.Add(new UICommand("Cancel", cmd => { }, commandId: 1));
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;
            var result = await dialog.ShowAsync();
            
        }
    }
}
