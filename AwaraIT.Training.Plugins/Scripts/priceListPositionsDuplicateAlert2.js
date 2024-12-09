function handleSaveError(executionContext) {
    var formContext = executionContext.getFormContext();
    //Получаем данные от пользователя
    var territoryId = formContext.getAttribute("fnt_territoryid").getValue()[0].id;
    var subjectId = formContext.getAttribute("fnt_subject").getValue()[0].id;
    var conductionFormatId = formContext.getAttribute("fnt_format_condaction")[0].id;
    var preparationFormatId = formContext.getAttribute("fnt_format_preparation")[0].id;
    var priceListName = formContext.getAttribute("fnt_price_list").getValue();
    var price = formContext.getAttribute("fnt_price").getValue();

    console.log("[handleSaveError] territoryId:", territoryId);
    console.log("[handleSaveError] subjectId:", subjectId);
    console.log("[handleSaveError] conductionFormatId:", conductionFormatId);
    console.log("[handleSaveError] preparationFormatId:", preparationFormatId);
    console.log("[handleSaveError] priceListName:", priceListName);
    console.log("[handleSaveError] price:", price);
    //формируем данные для запроса
    var parameters = {
        TerritoryId: {
            "@odata.type": "Microsoft.Dynamics.CRM.fnt_territory",
            fnt_territoryid: possibleDealId.replace('{', '').replace('}', '')
        },
        SubjectId: {
            "@odata.type": "Microsoft.Dynamics.CRM.fnt_study_subject",
            fnt_study_subjectid: subjectId.replace('{', '').replace('}', '')
        },
        ConductionFormatId: {
            "@odata.type": "Microsoft.Dynamics.CRM.fnt_event_type",
            fnt_event_typeid: conductionFormatId.replace('{', '').replace('}', '')
        },
        PreparationFormatId: {
            "@odata.type": "Microsoft.Dynamics.CRM.fnt_education_type",
            fnt_education_typeid: preparationFormatId.replace('{', '').replace('}', '')
        },
        PriceListName: priceListName,
        Price: price
    };

    console.log("[MyScript] Parameters for API request:", parameters);
    // подготавливаем запрос к api OData "/api/data/v9.0/fnt_CalculatePricingForPossibleDeal" в action
    var req = new XMLHttpRequest();
    req.open("POST", Xrm.Page.context.getClientUrl() + "/api/data/v9.0/fnt_CalculatePricingForPossibleDeal", true);
    req.setRequestHeader("OData-MaxVersion", "4.0");
    req.setRequestHeader("OData-Version", "4.0");
    req.setRequestHeader("Accept", "application/json"); //Accept- формат json
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.setRequestHeader("Prefer", "return=representation"); //отвечает за возврат результата обработки данных

    req.onreadystatechange = function () {
        // Проверяем, завершен ли запрос

        if (this.readyState === 4) {
            // Проверяем, был ли запрос успешным
            if (this.status === 200) {
                var result = JSON.parse(this.response);
                var basePrice = result.BasePrice;
                if () {
                    showInfo();
                }
            }
        }
    };
    //отправка запроса
    console.log("[MyScript] Sending API request...");
    req.send(JSON.stringify(parameters));
}

function showInfo() {
    console.log("[handleSaveError] showInfo function called");

    console.log("[handleSaveError] Error message:", errorMessage);
    eventArgs.preventDefault();
    var alertStrings = { confirmButtonLabel: "OK", text: "Позиция прайс-листа с такими данными уже существует. Пожалуйста, измените входные параметры.", title: "Ошибка" };
    var alertOptions = { height: 200, width: 400 };
    Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
        function () {
            console.log("[handleSaveError] Alert dialog closed");
        },
        function (error) {
            console.error("[handleSaveError] Error showing alert dialog:", error);
        }
    );
}


