using System;
using UnityEngine;

namespace StartApp
{
	public class StartAppWrapper : MonoBehaviour
	{
        public interface AdEventListener
        {
            void onReceiveAd();
            void onFailedToReceiveAd();
        }

        public interface AdDisplayListener
        {
            void adHidden();
            void adDisplayed();
            void adClicked();
        }

        private class ImplementationAdEventListener : AndroidJavaProxy
        {
            public ImplementationAdEventListener(AdEventListener listener) : base("com.startapp.android.publish.AdEventListener")
            {
                this.listener = listener;
            }

            private void onReceiveAd(AndroidJavaObject ad)
            {
                if (listener != null)
                {
                    listener.onReceiveAd();
                }
            }

            private void onFailedToReceiveAd(AndroidJavaObject ad)
            {
                if (listener != null)
                {
                    listener.onFailedToReceiveAd();
                }
            }

            private int hashCode()
            {
                return GetHashCode();
            }
            private AdEventListener listener;
        }

        private class ImplementationAdDisplayListener : AndroidJavaProxy
        {
            public ImplementationAdDisplayListener(AdDisplayListener listener) : base("com.startapp.android.publish.AdDisplayListener")
            {
                this.listener = listener;
            }

            private void adHidden(AndroidJavaObject ad)
            {
                if (listener != null)
                {
                    listener.adHidden();
                }
            }

            private void adDisplayed(AndroidJavaObject ad)
            {
                if (listener != null)
                {
                    listener.adDisplayed();
                }
            }

            private void adClicked(AndroidJavaObject ad)
            {
                if (listener != null)
                {
                    listener.adClicked();
                }
            }
            private AdDisplayListener listener;
        }

        private class OnBackPressedAdDisplayListener : AndroidJavaProxy
        {
            public OnBackPressedAdDisplayListener(string gameObjectName) : base("com.startapp.android.publish.AdDisplayListener")
            {
                this.gameObjectName = gameObjectName;
            }

            private void adHidden(AndroidJavaObject ad)
            {
                Debug.Log("ad hidden - quitting application");
                if (!clicked)
                {
                    init();
                    wrapper.Call("quit", new object[]
                    {
                        gameObjectName
                    });
                }
            }
            private void adDisplayed(AndroidJavaObject ad)
            {
            }

            private void adClicked(AndroidJavaObject ad)
            {
                clicked = true;
            }

            private string gameObjectName;
            private bool clicked;
        }

        public enum BannerPosition
        {
            BOTTOM,
            TOP
        }

        public enum BannerType
        {
            AUTOMATIC,
            STANDARD,
            THREED
        }

        private static string developerId;
        private static string applicatonId;
        private static AndroidJavaClass unityClass;
        private static AndroidJavaObject currentActivity;
        private static AndroidJavaObject wrapper;


        public static void loadAd(AdEventListener listener)
        {
            init();
            wrapper.Call("loadAd", new ImplementationAdEventListener[]
            {
                new ImplementationAdEventListener(listener)
            });
        }

        public static bool showAd(AdDisplayListener listener)
        {
            init();
            return wrapper.Call<bool>("showAd", new object[]
            {
                new ImplementationAdDisplayListener(listener)
            });
        }

        public static bool onBackPressed(string gameObjectName)
        {
            init();
            return wrapper.Call<bool>("onBackPressed", new object[]
            {
                new OnBackPressedAdDisplayListener(gameObjectName)
            });
        }

        public static void loadAd()
        {
            init();
            wrapper.Call("loadAd", new object[0]);
        }

        public static bool showAd()
        {
            init();
            return wrapper.Call<bool>("showAd", new object[0]);
        }

        public static void addBanner()
        {
            addBanner(BannerType.AUTOMATIC, BannerPosition.BOTTOM);
        }

        public static void addBanner(BannerType bannerType, BannerPosition position)
        {
            int num = 1;
            int num2 = 1;
            if (position != BannerPosition.BOTTOM)
            {
                if (position == BannerPosition.TOP)
                {
                    num = 2;
                }
            }
            else
            {
                num = 1;
            }
            AndroidJavaObject androidJavaObject = new AndroidJavaObject("java.lang.Integer", new object[]
            {
                num
            });
            switch (bannerType)
            {
                case BannerType.AUTOMATIC:
                    num2 = 1;
                    break;
                case BannerType.STANDARD:
                    num2 = 2;
                    break;
                case BannerType.THREED:
                    num2 = 3;
                    break;
            }
            AndroidJavaObject androidJavaObject2 = new AndroidJavaObject("java.lang.Integer", new object[]
            {
                num2
            });
            init();
            wrapper.Call("addBanner", new AndroidJavaObject[]
            {
                androidJavaObject2,
                androidJavaObject
            });
        }

        public static void removeBanner()
        {
            removeBanner(BannerPosition.BOTTOM);
        }

        public static void removeBanner(BannerPosition position)
        {
            int num = 1;
            if (position != BannerPosition.BOTTOM)
            {
                if (position == BannerPosition.TOP)
                {
                    num = 2;
                }
            }
            else
            {
                num = 1;
            }
           
            AndroidJavaObject androidJavaObject = new AndroidJavaObject("java.lang.Integer", new object[]
            {
                num
            });
            Debug.Log("masuk ke remove banner");
            init();
            wrapper.Call("removeBanner", new object[]
            {
                androidJavaObject
            });
            
        }

        public static void init()
        {
            if (wrapper == null)
            {
                initWrapper(false);
            }
        }

        private static void initWrapper(bool enableReturnAds)
        {
            unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
            wrapper = new AndroidJavaObject("com.startapp.android.unity.InAppWrapper", new object[]
            {
                currentActivity
            });
            if (!initUserData())
            {
                throw new ArgumentException("Error in initializing Application ID or Developer ID, Verify you initialized them in StartAppData in resources");
            }
            AndroidJavaObject androidJavaObject = new AndroidJavaObject("java.lang.String", new object[]
            {
                applicatonId
            });
            AndroidJavaObject androidJavaObject2 = new AndroidJavaObject("java.lang.String", new object[]
            {
                developerId
            });
            AndroidJavaObject androidJavaObject3 = new AndroidJavaObject("java.lang.Boolean", new object[]
            {
                enableReturnAds
            });
            wrapper.Call("init", new object[]
            {
                androidJavaObject2,
                androidJavaObject,
                androidJavaObject3
            });
        }

        private static bool initUserData()
        {
            bool result = false;
            int num = 0;
            TextAsset textAsset = (TextAsset)Resources.Load("StartAppData");
            string text = textAsset.ToString();
            string[] array = text.Split(new char[]
            {
                '\n'
            });
            for (int i = 0; i < array.Length; i++)
            {
                string[] array2 = array[i].Split(new char[]
                {
                    '='
                });
                if (array2[0].ToLower().CompareTo("applicationid") == 0)
                {
                    num++;
                    applicatonId = array2[1].ToString().Trim();
                }
                if (array2[0].ToLower().CompareTo("developerid") == 0)
                {
                    num++;
                    developerId = array2[1].ToString().Trim();
                }
            }
            if (num == 2)
            {
                result = true;
            }
            return result;
        }  
    }
}
