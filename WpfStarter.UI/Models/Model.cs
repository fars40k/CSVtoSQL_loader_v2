﻿using Prism.Ioc;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfStarter.Data;

namespace WpfStarter.UI.Models
{
    public partial class Model
    {
        public EnumGlobalState _appGlobalState { get; private set; }

        private DataFacade _dataFacade;
        private IContainerProvider _container;

        private string _sourceFile;

        public Action BeginOperation;
        public Action<string> AppStateChanged;
        public Action<string> FileSelected;

        public Model(IContainerProvider container)
        {
            _container = container;

            _dataFacade = container.Resolve<DataFacade>();
            SetDataViewsLocalisation(container);

            ApplyDefaultEventRouting();
            var dbWrk = container.Resolve<EntityWorker>();  
            DatabaseInitialized(dbWrk.DoesDatabaseConnectionInitialized);

        }

        private void DatabaseInitialized(bool IsInitialized)
        {
            if (IsInitialized)
            {
                SetAppGlobalState(EnumGlobalState.DbConnected);
            } else
            {
                SetAppGlobalState(EnumGlobalState.DbConnectionFailed);                
            }
        }

        public void ApplyDefaultEventRouting()
        {
            var dbWrk = _container.Resolve<EntityWorker>();
            FileSelected += (value) =>
            {
                if (new FileInfo(value).Length <= 64)
                {
                    ErrorNotify.NewError(Localisation.Strings.ErrorFileEmpty);
                }
                else
                {
                    SetAppGlobalState(EnumGlobalState.FileSelected);
                    dbWrk.AddDataViewsToRegions();
                    _sourceFile = value;
                }
            };

            BeginOperation += () =>
            {
                dbWrk.BeginOperation();
            };

        }
            private string GetAppGlobalState()
        {
            return _appGlobalState.ToString();
        }

        public void NotifyAppGlobalState()
        {
            if (AppStateChanged != null)
            {
                AppStateChanged.Invoke(_appGlobalState.ToString());
            }
        }

        public void SetAppGlobalState(EnumGlobalState newState)
        {
            _appGlobalState = newState;
            this.NotifyAppGlobalState();
        }

        protected void SetDataViewsLocalisation(IContainerProvider container)
        {
            var dVl = container.Resolve<DataViewsLocalisation>();
            dVl.SetNewViewString("Operation 1", Localisation.Strings.OpCSVtoSQLExist);
            dVl.SetNewViewString("Operation 2", Localisation.Strings.OpConvToXLSX);
            dVl.SetNewViewString("Operation 3", Localisation.Strings.OpConvToXML);
            dVl.SetNewViewString("Date", Localisation.Strings.FilterRow1);
            dVl.SetNewViewString("FirstName", Localisation.Strings.FilterRow2);
            dVl.SetNewViewString("LastName", Localisation.Strings.FilterRow3);
            dVl.SetNewViewString("SurName", Localisation.Strings.FilterRow4);
            dVl.SetNewViewString("City", Localisation.Strings.FilterRow5);
            dVl.SetNewViewString("Country", Localisation.Strings.FilterRow6);
            dVl.SetNewViewString("Filter 1", Localisation.Strings.FilterDataAll);
            dVl.SetNewViewString("Filter 2", Localisation.Strings.FilterDataContains);
        }
    }
}
