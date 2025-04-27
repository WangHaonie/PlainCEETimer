namespace PlainCEETimer.Modules.Win32Registry
{
    public class StartUp
    {
        /// <summary>
        /// 有关开机启动注册表的操作。0 - 检测状态，1 - 设置，2 - 删除。
        /// </summary>
        /// <param name="Operation"></param>
        /// <returns>null, <see cref="bool"/></returns>
        public object Operate(int Operation)
        {
            var KeyName = App.AppNameEngOld;
            var AppPath = $"\"{App.CurrentExecutablePath}\"";
            using var Helper = RegistryHelper.Open(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false);

            switch (Operation)
            {
                case 0: // state
                    return Helper.GetState(KeyName, AppPath, "");
                case 1: // set
                    Helper.Set(KeyName, AppPath);
                    return null;
                default: // delete
                    Helper.Delete(KeyName);
                    return null;
            }
        }
    }
}
