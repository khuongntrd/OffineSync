using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;

namespace MyApp.Helpers
{
    public static class Extensions
    {
        public static async Task<string> GetBase64ImageAsync(string filePath, Stream fileStream)
        {
            var bytes = new byte[fileStream.Length];

            await fileStream.ReadAsync(bytes, 0, (int)fileStream.Length);
            var base64 = Convert.ToBase64String(bytes);
            var mime = MimeTypes.GetMimeType(Path.GetFileName(filePath));
            return $"data:{mime};base64,{base64}";
        }


        private static async Task<bool> CheckPermissionStatusAsync(Permission permission)
        {

            var permissionStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(permission);
            var request = false;
            if (permissionStatus == PermissionStatus.Denied)
            {
                if (Device.RuntimePlatform == Device.iOS)
                {
                    var title = $"{permission} Permission";
                    var question =
                        $"To use this plugin the {permission} permission is required. Please go into Settings and turn on {permission} for the app.";
                    var positive = "Settings";
                    var negative = "Maybe Later";
                    var task = Application.Current?.MainPage?.DisplayAlert(title, question, positive, negative);

                    if (task == null) return false;

                    var result = await task;
                    if (result) CrossPermissions.Current.OpenAppSettings();

                    return false;
                }

                request = true;
            }

            if (request || permissionStatus != PermissionStatus.Granted)
            {
                var shouldShowRequestPermission = await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(permission);
                if (shouldShowRequestPermission)
                {
                    var newStatus = await CrossPermissions.Current.RequestPermissionsAsync(permission);
                    if (newStatus.ContainsKey(permission) && newStatus[permission] != PermissionStatus.Granted)
                    {
                        var title = $"{permission} Permission";
                        var question = $"To use the plugin the {permission} permission is required.";
                        var positive = "Settings";
                        var negative = "Maybe Later";
                        var task = Application.Current?.MainPage?.DisplayAlert(title, question, positive, negative);
                        if (task == null) return false;

                        var result = await task;
                        if (result) CrossPermissions.Current.OpenAppSettings();

                        return false;
                    }
                }
            }

            return true;
        }

        public static async Task<string> PickImageAsync(this Acr.UserDialogs.IUserDialogs dialogService, Image obj)
        {

            var result = await dialogService.ActionSheetAsync("Pick a photo", "Cancel", null, null, "From camera", "From gallery");

            if (result == "From gallery")
            {
                if (await CheckPermissionStatusAsync(Permission.Storage))
                {
                    await CrossMedia.Current.Initialize();

                    if (!CrossMedia.Current.IsPickPhotoSupported)
                    {
                        await dialogService.AlertAsync("Premission not granted", "Photos Not Supported", "OK");
                        return null;
                    }

                    var file = await CrossMedia.Current.PickPhotoAsync(
                        new PickMediaOptions
                        {
                            PhotoSize = PhotoSize.Medium,
                            CompressionQuality = 80
                        });

                    if (file == null)
                        return null;

                    return "file://" + file.Path;//, file.GetStream());
                }
            }
            else if (result == "From camera")
            {
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await dialogService.AlertAsync("No Camera", "No camera avaialble.", "OK");
                    return null;
                }
                if (await CheckPermissionStatusAsync(Permission.Camera))
                {
                    await CrossMedia.Current.Initialize();


                    var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        DefaultCamera = CameraDevice.Rear,
                        PhotoSize = PhotoSize.Small,
                        Directory = "my_app",
                        SaveToAlbum = true,
                    });

                    if (file == null)
                        return null;

                    return "file://" + file.Path;//, file.GetStream());
                }
            }
            return null;
        }
    }
}