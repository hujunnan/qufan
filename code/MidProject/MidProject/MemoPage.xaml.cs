using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace MidProject
{
    public sealed partial class MemoPage : Page
    {
        //共有部分
        public ViewModels.TodoItemListViewModel todoListVM = App.todoListViewModel;
        public ViewModels.TodoItemViewModel todoItemVM = App.todoItemViewModel;
        public ViewModels.MemorandumViewModel memorandunVM = App.memorandunViewModel;
        public ViewModels.PictureWallViewModel pictureVM = App.pictureWallViewModel;
        public ViewModels.BillViewModel billVM = App.billViewModel;

        //私有成员，判断是否是update
        private bool isUpdate = false;

        public MemoPage()
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

        //独有部分
        private void searchMemo(SearchBox sender, SearchBoxQuerySubmittedEventArgs e)
        {
            //应该是private async
            string searchdetail = searchbox.QueryText;
            if (searchdetail.Length == 0)
            {
                var i = new MessageDialog("搜索值为空").ShowAsync();
            }
            else
            {
                memorandunVM.search(searchdetail);
            }
        }

        private void addMemo(object sender, RoutedEventArgs e)
        {
            //增
            //判断是否为空，若不为空则添加，否则无操作
            //如果不是更新且text box不为空，则添加
            isUpdate = memorandunVM.SelectedMemorandum != null;
            if (textBox.Text != "" && !isUpdate)
            {
                //添加到memorandunVM
                memorandunVM.AddMemorandum(textBox.Text, DateTime.Now);
                //刷新页面
                Frame.Navigate(typeof(MemoPage));
            }
            //如果是更新且有text不为空，则更新
            else if (textBox.Text != "" && isUpdate)
            {
                //更新
                memorandunVM.UpdateMemorandum(memorandunVM.SelectedMemorandum.id, textBox.Text, DateTime.Now);
                //刷新页面
                Frame.Navigate(typeof(MemoPage));
            }
        }

        private void deleteMemo(object sender, RoutedEventArgs e)
        {
            //删
            //如果不是选中某条备忘录，则删除box内容
            //如果选中某条备忘录，则删除备忘录且清空box
            dynamic ori = e.OriginalSource;
            memorandunVM.SelectedMemorandum = (Models.Memorandum)ori.DataContext;
            isUpdate = memorandunVM.SelectedMemorandum != null;
            if (isUpdate)
            {
                textBox.Text = "";
                memorandunVM.RemoveMemorandum(memorandunVM.SelectedMemorandum.id);
                Frame.Navigate(typeof(MemoPage));
            }
            else
            {
                textBox.Text = "";
            }
        }

        //用于share
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
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
            var content = memorandunVM.SelectedMemorandum.content;
            request.Data.Properties.Title = "Memorandum";
            request.Data.Properties.Description = "From my memorandum";
            request.Data.SetText(content);
            deferral.Complete();
        }

        private void shareMemo(object sender, RoutedEventArgs e)
        {
            dynamic ori = e.OriginalSource;
            memorandunVM.SelectedMemorandum = (Models.Memorandum)ori.DataContext;
            DataTransferManager.ShowShareUI();
        }

        private void chooseAMemo(object sender, ItemClickEventArgs e)
        {
            var clickItem = (Models.Memorandum)(e.ClickedItem);//获取点击的item
            if (clickItem == memorandunVM.SelectedMemorandum)
            {
                textBox.Text = "";
                memorandunVM.SelectedMemorandum = null;
            }
            else
            {
                //跳转到memoDetailPage实现修改
                memorandunVM.SelectedMemorandum = clickItem;
                textBox.Text = memorandunVM.SelectedMemorandum.content;
            }
        }

        private void editMemo(object sender, RoutedEventArgs e)
        {
            dynamic ori = e.OriginalSource;
            memorandunVM.SelectedMemorandum = (Models.Memorandum)ori.DataContext;
            textBox.Text = memorandunVM.SelectedMemorandum.content;
        }

    }
}
