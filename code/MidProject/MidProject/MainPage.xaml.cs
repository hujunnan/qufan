using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace MidProject
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //每个界面都有的部分
        public ViewModels.TodoItemListViewModel todoListVM = App.todoListViewModel;
        public ViewModels.TodoItemViewModel todoItemVM = App.todoItemViewModel;
        public ViewModels.MemorandumViewModel memorandunVM = App.memorandunViewModel;
        public ViewModels.PictureWallViewModel pictureVM = App.pictureWallViewModel;
        public ViewModels.BillViewModel billVM = App.billViewModel;


        public MainPage()
        {
            this.InitializeComponent();

        }

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


        //本界面独有部分
        private void chooseAItem(object sender, ItemClickEventArgs e)
        {
            todoItemVM.SelectedItem = (Models.TodoItem)(e.ClickedItem);//获取点击的item
            //跳转到itemDetailPage实现修改
            App.isFromMain = true;
            Frame.Navigate(typeof(ItemDetailPage));
        }

        private void addItem(object sender, RoutedEventArgs e)//是添加item
        {
            //增 默认信息写在xaml注释部分
            todoItemVM.AddTodoItem(textbox.Text, DateTime.Today, DateTime.Today, true, null);
            //刷新页面
            Frame.Navigate(typeof(MainPage)); 
        }

        private void deleteItem(object sender, RoutedEventArgs e)
        {
            //删
            dynamic ori = e.OriginalSource;
            todoItemVM.SelectedItem = (Models.TodoItem)ori.DataContext;
            //判断是否有归属
            if (todoItemVM.SelectedItem.listid == null)
            {
                //直接删除
                todoItemVM.RemoveTodoItem(todoItemVM.SelectedItem.id);
                Frame.Navigate(typeof(MainPage));
            }
            else
            {
                //先在list里边删除，再在todoitem删除
                todoListVM.DeleteSingleItem(todoItemVM.SelectedItem.listid, todoItemVM.SelectedItem.id);
                todoItemVM.RemoveTodoItem(todoItemVM.SelectedItem.id);
                Frame.Navigate(typeof(MainPage));
            }
        }


        //用于share
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            todoItemVM.getToday();
            Frame rootFrame = Window.Current.Content as Frame;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            DataTransferManager.GetForCurrentView().DataRequested += OnShareDataRequested;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DataTransferManager.GetForCurrentView().DataRequested -= OnShareDataRequested;
        }

        private void OnShareDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var request = args.Request;
            var deferral = args.Request.GetDeferral();
            var content = "事件：" + todoItemVM.SelectedItem.jobName + " 提醒时间：" + todoItemVM.SelectedItem.remindAt
                        + " 到期时间：" + todoItemVM.SelectedItem.deadline + " 是否已完成:" + todoItemVM.SelectedItem.completed;
            request.Data.Properties.Title = "To Do Item";
            request.Data.Properties.Description = "From Today";
            request.Data.SetText(content);
            deferral.Complete();
        }

        private void shareItem(object sender, RoutedEventArgs e)
        {
            dynamic ori = e.OriginalSource;
            todoItemVM.SelectedItem = (Models.TodoItem)ori.DataContext;
            DataTransferManager.ShowShareUI();
        }

        private void searchItem(SearchBox sender, SearchBoxQuerySubmittedEventArgs e)
        {
            //应该是private async
            string searchdetail = searchbox.QueryText;
            if (searchdetail.Length == 0)
            {
                var i = new MessageDialog("搜索值为空").ShowAsync();
            }
            else
            {
                todoItemVM.search(searchdetail);
            }
        }

        private void checkBox_Click(object sender, RoutedEventArgs e)
        {
            dynamic ori = e.OriginalSource;
            todoItemVM.SelectedItem = (Models.TodoItem)ori.DataContext;
            todoItemVM.ChangeState(todoItemVM.SelectedItem.id);
            todoItemVM.SelectedItem = null;
            Frame.Navigate(typeof(MainPage));
        }
    }
}
