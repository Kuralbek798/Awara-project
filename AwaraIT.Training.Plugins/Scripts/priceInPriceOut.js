function onProductOrDiscountChange(executionContext) {
    var formContext = executionContext.getFormContext();
    //Получаем данные от пользователя
    var possibleDealId = formContext.getAttribute("fnt_posible_deal").getValue()[0].id;
    var productId = formContext.getAttribute("fnt_productid").getValue()[0].id;
    var discount = formContext.getAttribute("fnt_discount").getValue() || 0;

    console.log("[MyScript] Possible Deal ID:", possibleDealId);
    console.log("[MyScript] Product ID:", productId);
    console.log("[MyScript] Discount:", discount);
    //формируем данные для запроса
    var parameters = {
        PossibleDealId: {
            "@odata.type": "Microsoft.Dynamics.CRM.fnt_posible_deal",
            fnt_posible_dealid: possibleDealId.replace('{', '').replace('}', '')
        },
        ProductId: {
            "@odata.type": "Microsoft.Dynamics.CRM.fnt_education_product",
            fnt_education_productid: productId.replace('{', '').replace('}', '')
        },
        Discount: discount
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
        // 0: Запрос еще не инициализирован. 1: Запрос был инициализирован. 2: Запрос получен.
        // 3: Запрос в процессе обработки.4: Запрос завершен, и ответ полностью получен.
        if (this.readyState === 4) {
            // Проверяем, был ли запрос успешным
            if (this.status === 200) {
                var result = JSON.parse(this.response);
                var basePrice = result.BasePrice;
                var discountedPrice = result.DiscountedPrice;

                console.log("[MyScript] Base Price:", basePrice);
                console.log("[MyScript] Discounted Price:", discountedPrice);

                formContext.getAttribute("fnt_price").setValue(basePrice);
                formContext.getAttribute("fnt_price_after_discount").setValue(discountedPrice);
            } else {
                console.error("[MyScript] Request failed with status:", this.status);
                console.error("[MyScript] Error response:", this.response);
            }
        }
    };
    //отправка запроса
    console.log("[MyScript] Sending API request...");
    req.send(JSON.stringify(parameters));
}