using UnityEngine;

namespace Supernova.Utils
{
    public class CameraResolution : MonoBehaviour
    {
        private void Awake()
        {
            Camera camera = GetComponent<Camera>();
            int setWidth = 720; // ����� ���� �ʺ�
            int setHeight = 1560; // ����� ���� ����
            int deviceWidth = Screen.width;
            int deviceHeight = Screen.height;

            int safeDeviceWidth = (int)Screen.safeArea.width;
            int safeDeviceHeight = (int)Screen.safeArea.height;
            Rect safeArea = Screen.safeArea;

            Rect rect;
            if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // ����� �ػ� �� �� ū ���
            {
                float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // ���ο� �ʺ�
                rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // ���ο� Rect ����
            }
            else // ������ �ػ� �� �� ū ���
            {
                float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // ���ο� ����
                rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // ���ο� Rect ����
            }

            var x = rect.x * deviceWidth;
            var w = rect.width * deviceWidth;
            var y = rect.y * deviceHeight;
            var h = rect.height * deviceHeight;

            if (safeArea.x > x || safeArea.y > y || x + w > safeArea.x + safeArea.width || y + h > safeArea.y + safeArea.height)
            {
                Debug.Log("Out of safe Area");
                float xR = safeArea.x / deviceWidth;
                float yR = safeArea.y / deviceHeight;
                if ((float)setWidth / setHeight < (float)safeDeviceWidth / safeDeviceHeight) // ����� �ػ� �� �� ū ���
                {
                    var rateTemp = (float)safeDeviceWidth / deviceWidth;
                    float newWidth = ((float)setWidth / setHeight) / ((float)safeDeviceWidth / safeDeviceHeight); // ���ο� �ʺ�

                    rect = new Rect(xR + ((1f - newWidth) / 2f) * rateTemp, yR, newWidth * rateTemp, 1f - yR); // ���ο� Rect ����
                }
                else // ������ �ػ� �� �� ū ���
                {
                    var rateTemp = (float)safeDeviceHeight / deviceHeight;
                    float newHeight = ((float)safeDeviceWidth / safeDeviceHeight) / ((float)setWidth / setHeight); // ���ο� ����
                    rect = new Rect(xR, yR + ((1f - newHeight) / 2f) * rateTemp, 1f - xR, newHeight * rateTemp); // ���ο� Rect ����
                }
            }

            camera.rect = rect;
        }

        void OnPreCull() => GL.Clear(false, false, Color.black);
    }
}
