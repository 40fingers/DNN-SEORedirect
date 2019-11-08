var SR = SR || {};

SR.UnhandledUrlVm = function(data) {
    // reference to self
    var self = this;

    // create observables from data
    ko.mapping.fromJS(data, null, self);

    self.isLoading = ko.observable(false);
    self.isHandled = ko.observable(false);
    self.isBinding = ko.observable(true);
    self.index = ko.observable(0);
    self.isAddRedirect = ko.observable(false);
    self.internalTargetUrl = ko.observable("");
    self.targetUrl = ko.observable("");
    self.targetTabName = ko.observable("");
    self.targetTabId = ko.observable(-1);
    self.PageDropDownInitialized = ko.observable(false);

    self.mapToType = ko.observable("NONE");

    self.mapToUrl = ko.computed(function() {
        var retval = !self.mapToPage() && self.targetUrl() !== null && self.targetUrl().length > 0;
        return retval;
    });
    self.mapToPage = ko.computed(function() {
        var retval = self.targetTabId() !== null &&
            self.targetTabId() > 0 &&
            (self.targetTabName() === null || self.targetTabName() !== "");
        return retval;
    });
    self.mapToNone = ko.computed(function() {
        var retval = !(self.mapToUrl() || self.mapToPage());
        return retval;
    });

    self.initMapToType = function() {
        if (self.mapToUrl()) {
            FF.log(self.id() + " setting to URL");
            self.mapToType("URL");
        }
        if (self.mapToPage()) {
            FF.log(self.id() + " setting to TAB");
            self.mapToType("TAB");
        }
    };

    self.getTargetUrl = function() {
        if (self.mapToUrl()) {
            return self.internalTargetUrl();
        }
        return "";
    };
    //self.getTargetTabId = function() {
    //    if (self.mapToPage()) {
    //        // looks like this: 
    //        // {"selectedItem":{"key":"155","value":"Geeltjes"}}
    //        var ddlData = FF.parseJSON($("#moveablePageDropDown input[type='hidden']").val());
    //        return ddlData.selectedItem.key;
    //    }
    //    return 0;
    //}

    self.saveRedirect = function() {
        var postData = {};
        postData.SourceUrl = self.url();
        //postData.UseRegex = self.useRegex();

        var maptotype = self.mapToType();

        switch (maptotype) {
        case "URL":
            postData.TargetTabId = -1;
            postData.TargetUrl = self.targetUrl();
            break;
        case "TAB":
            postData.TargetTabId = self.targetTabId();
            postData.TargetUrl = "";
            break;
        default:
            postData.TargetTabId = -1;
            postData.TargetUrl = "";
            break;
        }

        var url = SR.service.baseUrl + "SaveRedirect"; //"?sourceUrl=" + encodeURIComponent(self.url());
        //if (self.mapToUrl()) {
        //    url = url + "&targetTabId=0&targetUrl=" + encodeURIComponent(self.getTargetUrl());
        //} else if (self.mapToPage()) {
        //    url = url + "&targetUrl=&targetTabId=" + self.targetTabId();
        //} else {
        //    url = url + "&targetUrl=&targetTabId=0";
        //}

        self.isLoading(true);
        var jqXHR = $.ajax({
            url: url,
            beforeSend: SR.service.setModuleHeaders,
            type: "POST",
            data: postData,
            dataType: "json"
        }).done(function(data) {
            self.toggleAddRedirectPanel();
            self.isHandled(true);
        }).always(function(data) {
            self.isLoading(false);
        });

        return false;
    };

    self.toggleAddRedirectPanel = function() {
        var panelSelector = "#addRedirectPanel_" + self.index().toString();
        var showImgSelector = "#showAddRedirect_" + self.index().toString();
        var hideImgSelector = "#hideAddRedirect_" + self.index().toString();
        var pagedropdownContainerSelector = "#redirectPageDropdown_" + self.index().toString();

        var createdropdownContainerSelector = "#createPageDropdown_" + self.index().toString();
        var createdropdownContainerStateId = "createPageDropdownState_" + self.index().toString();

        var isVisible = FF.isVisible(panelSelector);
        if (isVisible) {
            FF.hide(panelSelector);
            FF.hide(hideImgSelector);
            FF.show(showImgSelector);
        } else {
            // first hide all other panels
            FF.hide(".ffrs-addRedirectPanel");
            // then show the right one
            FF.show(panelSelector);
            FF.show(hideImgSelector);
            FF.hide(showImgSelector);

            if (self.PageDropDownInitialized() === false) {
                self.PageDropDownInitialized(true);
                var initialstate = { "selectedItem": null };
                //FF.log("Trying to create dropdown in " + createdropdownContainerSelector + " with initialstate: " + JSON.stringify(initialstate));

                dnn.createDropDownList(createdropdownContainerSelector,
                    {
                        "selectedItemCss": "selected-item",
                        "internalStateFieldId": createdropdownContainerStateId,
                        "disabled": false,
                        "selectItemDefaultText": "Select A Web Page",
                        "initialState": initialstate,
                        "services": {
                            "moduleId": srMid.toString(),
                            "serviceRoot": "InternalServices",
                            "getTreeMethod": "ItemListService/GetPages",
                            "sortTreeMethod": "ItemListService/SortPages",
                            "getNodeDescendantsMethod": "ItemListService/GetPageDescendants",
                            "searchTreeMethod": "ItemListService/SearchPages",
                            "getTreeWithNodeMethod": "ItemListService/GetTreePathForPage",
                            "rootId": "Root",
                            "parameters": {
                                "PortalId": "" + srPid.toString() + "",
                                "includeDisabled": "true",
                                "includeAllTypes": "false",
                                "includeActive": "false",
                                "includeHostPages": "false",
                                "roles": ""
                            }
                        },
                        "itemList": {
                            "sortAscendingButtonTitle": "A-Z",
                            "unsortedOrderButtonTooltip": "Remove sorting",
                            "sortAscendingButtonTooltip": "Sort in ascending order",
                            "sortDescendingButtonTooltip": "Sort in descending order",
                            "selectedItemExpandTooltip": "Click to expand",
                            "selectedItemCollapseTooltip": "Click to collapse",
                            "searchInputPlaceHolder": "Search...",
                            "clearButtonTooltip": "Clear",
                            "searchButtonTooltip": "Search",
                            "loadingResultText": "...Loading Results",
                            "resultsText": "Results",
                            "firstItem": null,
                            "disableUnspecifiedOrder": false
                        },
                        "onSelectionChanged": ["ff_seo_selectedPageChanged"]
                    },
                    {});
            }

        }
        return false;
    };

    self.setSelectedTab = function(currentVal, event) {
        FF.log("setselectedtab");
        var selItem = FF.parseJSON(event.currentTarget.value);
        var tabid = -1;
        var tabName = "";
        if (typeof (selItem) === "object") {
            var kvPair = selItem.selectedItem;
            if (kvPair !== null) {
                tabid = parseInt(kvPair.key);
                tabName = kvPair.value;
            }
        }

        self.targetTabId(tabid);
        self.targetTabName(tabName);

        self.mapToType('TAB');

        //$(event.currentTarget).val(event.currentTarget.value);
    };
};

SR.UnhandledUrlsVm = function() {
    // reference to self
    var self = this;

    var urlIndexCounter = 0;
    self.mapping = {
        'urls': {
            create: function(options) {
                var retval = new SR.UnhandledUrlVm(options.data);
                retval.index(urlIndexCounter);
                urlIndexCounter++;
                return retval;
            }
        }
    };

    self.isLoading = ko.observable(false);
    self.bindingsApplied = ko.observable(false);

    self.load = function(numUrls) {
        self.isLoading(true);
        var jqXHR = $.ajax({
            url: SR.service.baseUrl + "GetUnhandledUrls",
            beforeSend: SR.service.setModuleHeaders,
            dataType: "json"
        }).done(function(data) {
            if (data) {
                // we maken nieuwe properties van de data
                ko.mapping.fromJS(data, self.mapping, self);

                if (!self.bindingsApplied()) {
                    self.bindingsApplied(true);
                    ko.applyBindings(self, $("#koUnhandledUrlsWrapper")[0]);
                }
            }
        }).always(function(data) {
            self.isLoading(false);
        });
    };

    self.removeItem = function(item) {
        self.urls.remove(item);
    };
};

SR.MappingVm = function(data) {
    // reference to self
    var self = this;

    // create observables from data
    ko.mapping.fromJS(data, null, self);

    self.isLoading = ko.observable(false);
    self.isRemoved = ko.observable(false);
    self.isBinding = ko.observable(true);
    self.index = ko.observable(0);
    self.PageDropDownInitialized = ko.observable(false);
    self.mapToType = ko.observable("NONE");

    self.mapToUrl = ko.computed(function() {
        // we have a url and no tabid
        var retval = self.targetUrl() !== null &&
            self.targetUrl().length > 0 &&
            (self.targetTabId() === null || self.targetTabId() <= 0);
        return retval;
    });
    self.mapToPage = ko.computed(function() {
        // we have a tabid and a tabname (which means the tabid exists)
        var retval = self.targetTabId() !== null &&
            self.targetTabId() > 0 &&
            (self.targetTabName() === null || self.targetTabName() !== "");
        return retval;
    });
    self.mapToNone = ko.computed(function() {
        var retval = !(self.mapToUrl() || self.mapToPage());
        return retval;
    });

    self.initMapToType = function() {
        if (self.mapToUrl()) {
            FF.log(self.id() + " setting to URL");
            self.mapToType("URL");
        }
        if (self.mapToPage()) {
            FF.log(self.id() + " setting to TAB");
            self.mapToType("TAB");
        }
    };

    //var summary = {};
    //summary.id = self.id();
    //summary.mapToUrl = self.mapToUrl();
    //summary.mapToPage = self.mapToPage();
    //summary.mapToType = self.mapToType();
    //summary.targetTabId = self.targetTabId();
    //summary.targetTabName = self.targetTabName();
    //summary.targetUrl = self.targetUrl();
    //FF.log(JSON.stringify(summary));


    self.saveMapping = function() {
        FF.log(self.id() + " saving with maptotype " + self.mapToType());

        var postData = {};
        postData.Id = self.id();
        postData.SourceUrl = self.sourceUrl();
        postData.UseRegex = self.useRegex();

        var maptotype = self.mapToType();

        switch (maptotype) {
        case "URL":
            postData.TargetTabId = -1;
            postData.TargetUrl = self.targetUrl();
            break;
        case "TAB":
            postData.TargetTabId = self.targetTabId();
            postData.TargetUrl = "";
            break;
        default:
            postData.TargetTabId = -1;
            postData.TargetUrl = "";
            break;
        }


        self.isLoading(true);
        var jqXHR = $.ajax({
            url: SR.service.baseUrl + "SaveMapping",
            type: "POST",
            data: postData,
            beforeSend: SR.service.setModuleHeaders,
            dataType: "json"
        }).done(function(data) {
            if (data === null) {
                // this item should be removed
                self.toggleEditMappingPanel();
                self.isRemoved(true);
                return;
            }

            self.toggleEditMappingPanel();

            var obj = FF.parseJSON(data);
            self.id(obj.id);
            self.sourceUrl(obj.sourceUrl);
            self.useRegex(obj.useRegex);
            self.targetUrl(obj.targetUrl);
            self.targetTabId(obj.targetTabId);
            self.initMapToType();

        }).always(function(data) {
            self.isLoading(false);
        });

        return false;
    };

    self.toggleEditMappingPanel = function () {
        FF.log("toggleEditMappingPanel " + self.index().toString());
        var panelSelector = "#editMappingPanel_" + self.index().toString();
        var showImgSelector = "#showEditMapping_" + self.index().toString();
        var hideImgSelector = "#hideEditMapping_" + self.index().toString();
        var pagedropdownContainerSelector = "#redirectPageDropdown_" + self.index().toString();
        var createdropdownContainerSelector = "#createPageDropdown_" + self.index().toString();
        var createdropdownContainerStateId = "createPageDropdownState_" + self.index().toString();
        var isVisible = FF.isVisible(panelSelector);
        if (isVisible) {
            FF.hide(panelSelector);
            FF.hide(hideImgSelector);
            FF.show(showImgSelector);
        } else {
            // make sure the maptotype is correct before showing
            self.initMapToType();

            // first hide all other panels
            FF.hide(".ffrs-editMappingPanel");
            // hide cancel button images
            FF.hideNshow(".imgHideEditMapping", ".imgShowEditMapping", null);
            // then show the right one
            FF.show(panelSelector);
            FF.show(hideImgSelector);
            FF.hide(showImgSelector);

            //$("#moveablePageDropDown").appendTo(pagedropdownContainerSelector);
            //$("#moveablePageDropDown").removeClass("hidden");

            if (self.PageDropDownInitialized() === false) {
                self.PageDropDownInitialized(true);
                var initialstate = { "selectedItem": null };
                if (self.mapToPage() && self.targetTabName() !== "") {
                    initialstate.selectedItem = { "key": self.targetTabId().toString(), "value": self.targetTabName() };
                }
                //FF.log("Trying to create dropdown in " + createdropdownContainerSelector + " with initialstate: " + JSON.stringify(initialstate));

                dnn.createDropDownList(createdropdownContainerSelector,
                    {
                        "selectedItemCss": "selected-item",
                        "internalStateFieldId": createdropdownContainerStateId,
                        "disabled": false,
                        "selectItemDefaultText": "Select A Web Page",
                        "initialState": initialstate,
                        "services": {
                            "moduleId": srMid.toString(),
                            "serviceRoot": "InternalServices",
                            "getTreeMethod": "ItemListService/GetPages",
                            "sortTreeMethod": "ItemListService/SortPages",
                            "getNodeDescendantsMethod": "ItemListService/GetPageDescendants",
                            "searchTreeMethod": "ItemListService/SearchPages",
                            "getTreeWithNodeMethod": "ItemListService/GetTreePathForPage",
                            "rootId": "Root",
                            "parameters": {
                                "PortalId": "" + srPid.toString() + "",
                                "includeDisabled": "true",
                                "includeAllTypes": "false",
                                "includeActive": "false",
                                "includeHostPages": "false",
                                "roles": ""
                            }
                        },
                        "itemList": {
                            "sortAscendingButtonTitle": "A-Z",
                            "unsortedOrderButtonTooltip": "Remove sorting",
                            "sortAscendingButtonTooltip": "Sort in ascending order",
                            "sortDescendingButtonTooltip": "Sort in descending order",
                            "selectedItemExpandTooltip": "Click to expand",
                            "selectedItemCollapseTooltip": "Click to collapse",
                            "searchInputPlaceHolder": "Search...",
                            "clearButtonTooltip": "Clear",
                            "searchButtonTooltip": "Search",
                            "loadingResultText": "...Loading Results",
                            "resultsText": "Results",
                            "firstItem": null,
                            "disableUnspecifiedOrder": false
                        },
                        "onSelectionChanged": ["ff_seo_selectedPageChanged"]
                    },
                    {});
            }
            //debugger;
            //var selectedItem = null;
            //$("#moveablePageDropDown .dnnDropDownList").DropDownList("selectedItem", selectedItem);
        }
    };

    self.setSelectedTab = function(currentVal, event) {
        FF.log("setselectedtab");
        var selItem = FF.parseJSON(event.currentTarget.value);
        var tabid = -1;
        var tabName = "";
        if (typeof (selItem) === "object") {
            var kvPair = selItem.selectedItem;
            if (kvPair !== null) {
                tabid = parseInt(kvPair.key);
                tabName = kvPair.value;
            }
        }

        self.targetTabId(tabid);
        self.targetTabName(tabName);

        self.mapToType('TAB');

        //$(event.currentTarget).val(event.currentTarget.value);
    };

    return self;
};

SR.MappingsVm = function() {
    // reference to self
    var self = this;

    var urlIndexCounter = 0;
    self.mapping = {
        'mappings': {
            create: function(options) {
                var retval = new SR.MappingVm(options.data);
                retval.index(urlIndexCounter);
                urlIndexCounter++;
                return retval;
            }
        }
    };

    self.isLoading = ko.observable(false);
    self.bindingsApplied = ko.observable(false);

    self.load = function() {
        self.isLoading(true);
        var jqXHR = $.ajax({
            url: SR.service.baseUrl + "GetMappings",
            beforeSend: SR.service.setModuleHeaders,
            dataType: "json"
        }).done(function(data) {
            if (data) {
                // we maken nieuwe properties van de data
                ko.mapping.fromJS(data, self.mapping, self);

                if (!self.bindingsApplied()) {
                    self.bindingsApplied(true);
                    ko.applyBindings(self, $("#koEditMappingsWrapper")[0]);
                }
            }
        }).always(function(data) {
            self.isLoading(false);
        });
    };

    self.addMapping = function() {
        FF.log("AddMapping");

        var newMap = new SR.MappingVm(
            {
                "id": "",
                "sourceUrl": "",
                "targetUrl": "",
                "targetTabId": -1,
                "targetTabName": "",
                "useRegex": false
            });
        newMap.index(0);
        self.mappings.unshift(newMap);
        var i;
        for (i = 1; i < self.mappings().length; i++) {
            FF.log("Changing " + self.mappings()[i].id() + ' to ' + i.toString());
            self.mappings()[i].index(i);
        }
        newMap.toggleEditMappingPanel();
    };

    self.removeItem = function(mappingModel) {
        FF.log("About to remove item " + mappingModel.id());
        self.mappings.remove(mappingModel);
    };
};
