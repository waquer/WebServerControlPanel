using System;
using System.Windows.Input;

namespace WebServerControlPanel.Utils
{
    public class ScCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="execute">执行操作的委托</param>
        /// <param name="canExecute">判断是否可执行的委托（可选）</param>
        /// 
        public ScCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="parameter">传递给命令的参数</param>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        /// 判断命令是否可用
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <returns>布尔值</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// 当 CanExecute 变化时触发的事件
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
