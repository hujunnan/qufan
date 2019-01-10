using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
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
    public sealed partial class PictureDetailPage : Page
    {
        public PictureDetailPage()
        {
            this.InitializeComponent();
        }
        
        //每个界面都有的部分
        public ViewModels.TodoItemListViewModel todoListVM = App.todoListViewModel;
        public ViewModels.TodoItemViewModel todoItemVM = App.todoItemViewModel;
        public ViewModels.MemorandumViewModel memorandunVM = App.memorandunViewModel;
        public ViewModels.PictureWallViewModel pictureVM = App.pictureWallViewModel;
        public ViewModels.BillViewModel billVM = App.billViewModel;
        private BitmapImage tempPic;

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
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (pictureVM.SelectedPictureWall != null)
            {
                myImage.ImageSource = pictureVM.SelectedPictureWall.image;
                Description.Text = pictureVM.SelectedPictureWall.description;
                tempPic = (BitmapImage)myImage.ImageSource;
            }
        }

        private void addPW(object sender, RoutedEventArgs e)
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

        private void clear(object sender, RoutedEventArgs e)
        {
            myImage.ImageSource = null;
            Description.Text = "";
        }

        private void deletePW(object sender, RoutedEventArgs e)
        {
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

        private async void choosePicture(object sender, RoutedEventArgs e)
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
    }
}
