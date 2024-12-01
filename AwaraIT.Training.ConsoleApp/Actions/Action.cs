/*using AwaraIT.Kuralbek.Plugins.PluginExtensions;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;

namespace AwaraIT.Kuralbek.Plugins.Plugin
{
    /// <summary>
    /// Класс <c>Action</c> представляет действие в рабочем процессе CRM.
    /// </summary>
    public class Action : BasicActivity
    {
        /// <summary>
        /// Входной параметр теста.
        /// </summary>
        [Input("InputTest")]
        public InArgument<string> InputTest { get; set; }

        /// <summary>
        /// Выходной параметр сообщения об ошибке.
        /// </summary>
        [Output("ErrorMessage")]
        public OutArgument<string> ErrorMessage { get; set; }

        /// <summary>
        /// Метод, выполняющий логику действия.
        /// </summary>
        /// <param name="executionContext">Контекст выполнения действия.</param>
        protected override void Execute(CodeActivityContext executionContext)
        {
            base.Execute(executionContext);

            var inputTest = InputTest.Get(executionContext);
            ErrorMessage.Set(executionContext, "");

            try
            {
                Logger.INFO("Action TestAction", $"Ok: {DateTime.UtcNow}. {inputTest}");
            }
            catch (Exception ex)
            {
                Logger.ERROR("Action TestAction", ex.ToString());
                ErrorMessage.Set(executionContext, ex.Message);
            }
        }
    }
}





*/