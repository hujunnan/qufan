using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace MidProject
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class PicturePage : Page
    {
        //共有部分
        public ViewModels.TodoItemListViewModel todoListVM = App.todoListViewModel;
        public ViewModels.TodoItemViewModel todoItemVM = App.todoItemViewModel;
        public ViewModels.MemorandumViewModel memorandunVM = App.memorandunViewModel;
        public ViewModels.PictureWallViewModel pictureVM = App.pictureWallViewModel;
        public ViewModels.BillViewModel billVM = App.billViewModel;

        //判断是否要跳转的界面的大小
        int adaptiveWidth = 600;
        //记录bitmap
        BitmapImage tempPic = null;

        public PicturePage()
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
            var i = new MessageDialog("奴才正在努力开发中，请小主稍候").ShowAsync();
            //弹个窗说该功能尚未实现敬请期待
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
        private void chooseAPicture(object sender, ItemClickEventArgs e)
        {
            //之前未选中
            if (pictureVM.SelectedPictureWall != (Models.PictureWall)(e.ClickedItem))
            {
                pictureVM.SelectedPictureWall = (Models.PictureWall)(e.ClickedItem);
                //如果界面过小，就界面跳转
                if (this.RenderSize.Width <= adaptiveWidth)
                {
                    Frame.Navigate(typeof(PictureDetailPage), pictureVM);
                }
                else
                {
                    myImage.ImageSource = pictureVM.SelectedPictureWall.image;
                    Description.Text = pictureVM.SelectedPictureWall.description;
                    tempPic = (BitmapImage)myImage.ImageSource;
                }
            }
            //之前已经选中
            else
            {
                if (this.RenderSize.Width <= adaptiveWidth)
                {
                    Frame.Navigate(typeof(PictureDetailPage), pictureVM);
                }
                else
                {
                    pictureVM.SelectedPictureWall = null;
                    myImage.ImageSource = null;
                    Description.Text = "";
                    tempPic = null;
                }
            }
        }

        private async void openFolder(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
                BitmapImage bmp = new BitmapImage();
                bmp.SetSource(stream);
                myImage.ImageSource = bmp;
                StorageFile sampleFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("myMemory.jpg", CreationCollisionOption.GenerateUniqueName);
                await file.CopyAndReplaceAsync(sampleFile);
                bmp.UriSource = new Uri("ms-appdata:///Local/" + sampleFile.Name);
                tempPic = bmp;
            }
        }

        private void addPW(object sender, RoutedEventArgs e)
        {
            //增
            //判断是否要跳转，如果不需要跳转，判断是否为更新，如果不用更新，判断图片是否为空，如果非空，则添加
            //如果是更新，重置内容
            if (this.RenderSize.Width <= adaptiveWidth)
            {
                Frame.Navigate(typeof(PictureDetailPage), pictureVM);
            }
            else
            {
                bool isUpdate = pictureVM.SelectedPictureWall != null;
                if (isUpdate)
                {
                    pictureVM.UpdatePictureWall(pictureVM.SelectedPictureWall.id, Description.Text, DateTime.Now, tempPic);
                    //刷新界面
                    Frame.Navigate(typeof(PicturePage));
                }
                else
                {
                    if (tempPic != null)
                    {
                        pictureVM.AddPictureWall(Description.Text, DateTime.Now, tempPic);
                        //刷新界面
                        Frame.Navigate(typeof(PicturePage));
                    }
                    else
                    {
                        var i = new MessageDialog("请添加图片").ShowAsync();
                    }
                }
            }
        }

        private void deletePW(object sender, RoutedEventArgs e)
        {
            //删
            //判断selected item是否为空，若非空，删除item，并清空信息
            dynamic ori = e.OriginalSource;
            pictureVM.SelectedPictureWall = (Models.PictureWall)ori.DataContext;
            bool isUpdate = pictureVM.SelectedPictureWall != null;
                if (isUpdate)
                {
                    pictureVM.RemovePictureWall(pictureVM.SelectedPictureWall.id);
                    //刷新界面
                    Frame.Navigate(typeof(PicturePage));
                }
                else
                {
                    myImage.ImageSource = null;
                    Description.Text = "";
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

        private async void OnShareDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var request = args.Request;
            var deferral = args.Request.GetDeferral();
            var content = pictureVM.SelectedPictureWall.description;
            var photoFile = await StorageFile.GetFileFromApplicationUriAsync(pictureVM.SelectedPictureWall.image.UriSource);
            request.Data.Properties.Title = "Picture";
            request.Data.Properties.Description = "From my memory";
            request.Data.SetStorageItems(new List<StorageFile> { photoFile });
            request.Data.SetText(content);
            deferral.Complete();
        }

        private void sharePW(object sender, RoutedEventArgs e)
        {
            dynamic ori = e.OriginalSource;
            pictureVM.SelectedPictureWall = (Models.PictureWall)ori.DataContext;
            DataTransferManager.ShowShareUI();
        }

        private void searchPW(SearchBox sender, SearchBoxQuerySubmittedEventArgs e)
        {
            //应该是private async
            string searchdetail = searchbox.QueryText;
            if (searchdetail.Length == 0)
            {
                var i = new MessageDialog("搜索值为空").ShowAsync();
            }
            else
            {
                pictureVM.search(searchdetail);
            }
        }

        private void editPW(object sender, RoutedEventArgs e)
        {
            dynamic ori = e.OriginalSource;
            pictureVM.SelectedPictureWall = (Models.PictureWall)ori.DataContext;
            if (this.RenderSize.Width <= adaptiveWidth)
            {
                Frame.Navigate(typeof(PictureDetailPage), pictureVM);
            }
            else
            {
                myImage.ImageSource = pictureVM.SelectedPictureWall.image;
                Description.Text = pictureVM.SelectedPictureWall.description;
            }
        }
    }
}
