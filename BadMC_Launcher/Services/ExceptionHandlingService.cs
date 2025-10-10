namespace BadMC_Launcher.Services;
internal class ExceptionHandlingService {
    public void ToastException(string title, string message, Exception? exception = null, Action? action = null) {
        action?.Invoke();
        if (exception != null) {
            Debug.WriteLine("《 你 码 崩 了 》" + exception.Message);
        }
        //TODO: 下世纪搞一下1吐司2框框（嘿嘿）
    }

    public void DialogException(string title, string message, Exception? exception = null, Action? action = null) {
        action?.Invoke();
        Debug.WriteLine("《 你 码 崩 了 》" + exception?.Message);
        //TODO: 下世纪搞一下1吐司2框框（嘿嘿）
    }
}
