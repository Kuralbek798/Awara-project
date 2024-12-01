using AwaraIT.Training.Application.Core;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;

namespace AwaraIT.Kuralbek.Plugins.PluginExtensions
{
    /// <summary>
    /// Абстрактный класс <c>BasicActivity</c> представляет базовую активность для рабочих процессов в CRM.
    /// </summary>
    public abstract class BasicActivity : CodeActivity
    {
        /// <summary>
        /// Сервис организации CRM.
        /// </summary>
        public IOrganizationService CrmService { get; set; }

        /// <summary>
        /// Контекст рабочего процесса.
        /// </summary>
        internal IWorkflowContext WorkflowContext { get; set; }

        /// <summary>
        /// Логгер для записи журналов.
        /// </summary>
        public Logger Logger { get; set; }

        /// <summary>
        /// Метод, выполняющий логику активности.
        /// </summary>
        /// <param name="executionContext">Контекст выполнения активности.</param>
        protected override void Execute(CodeActivityContext executionContext)
        {
            // Получение контекста рабочего процесса
            var context = executionContext.GetExtension<IWorkflowContext>();
            // Получение фабрики сервисов организации
            var serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            // Создание сервиса организации
            var service = serviceFactory.CreateOrganizationService(context.UserId);

            // Инициализация свойств
            CrmService = service;
            WorkflowContext = context;
            try
            {
                // Инициализация логгера
                Logger = new Logger(service);
            }
            catch { }
        }
    }
}





