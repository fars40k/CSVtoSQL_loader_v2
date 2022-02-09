using Microsoft.Win32;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using WpfStarter.Data.Export;

namespace WpfStarter.Data
{
    public class DefaultPreprocessor : IPreprocessor
    {
        private Dictionary<Type, Action> preprocessScenarious;
        private IContainerExtension _container;

        private DataAccessModel _model;
        private ResourceManager _resourceManager;

        private IDatabaseAction _databaseAction;

        public DefaultPreprocessor(IContainerExtension container)
        {
            _container = container;
            _model = _container.Resolve<DataAccessModel>();
            _resourceManager = _container.Resolve<ResourceManager>();

            preprocessScenarious.Add(typeof(IRequiringBuildLinq), BuildLinq);
            preprocessScenarious.Add(typeof(IRequiringSavepathSelection), SelectSavepath);
            preprocessScenarious.Add(typeof(IRequiringSourceFileSelection), SelectSourcefile);

        }

        public void PreprocessByImplementationsInTheAction()
        {

            _databaseAction = _model.SelectedAction;
            if (_databaseAction == null)
            {
                _model.NotifyMessageFromData.Invoke(_resourceManager.GetString("OpNotSelected"));
            }

            try
            {
                Type[] implementedCollection = _databaseAction.GetType().GetInterfaces();
                foreach (Type type in implementedCollection)
                {
                    if (preprocessScenarious.ContainsKey(type)) preprocessScenarious[type].Invoke();
                }

                // Creates new instances for class fields in a selected operation
                if ((_databaseAction is IParametrisedAction<Inference>))
                {
                    IParametrisedAction<Inference> action = (IParametrisedAction<Inference>)_databaseAction;
                    Inference obj = _container.Resolve<Inference>();
                    action.Settings = obj;
                }

            }
            catch (Exception ex)
            {
                _model.NotifyMessageFromData.Invoke(ex.Message);
            }
        }




        private void SelectSourcefile()
        {
            IRequiringSourceFileSelection sourceFileService = (IRequiringSourceFileSelection)_databaseAction;
            sourceFileService.SourceFilePath = _model.SourceFile;
        }

        private void SelectSavepath()
        {
            IRequiringSavepathSelection savepathService = (IRequiringSavepathSelection)_databaseAction;
            SaveFileDialog dialog = new SaveFileDialog();
            Random random = new Random();

            dialog.DefaultExt = savepathService.TargetFormat;
            dialog.FileName = random.Next(0, 9000).ToString();
            dialog.Filter = "(*" + dialog.DefaultExt + ")|*" + dialog.DefaultExt;
            dialog.ShowDialog();

            if ((dialog.FileName != "") && (dialog.FileName.Contains("." + dialog.DefaultExt)))
            {

                // Checks if a file exist, then, to prevent override, set new savepath to save
                // extracted data in incremental marked file

                string nonDuplicatefilePath = dialog.FileName;
                if (File.Exists(dialog.FileName))
                {
                    int increment = 0;

                    while (File.Exists(dialog.FileName))
                    {
                        increment++;
                        dialog.FileName = nonDuplicatefilePath
                                          .Replace(".", ("_" + increment.ToString() + "."));
                    }
                    nonDuplicatefilePath = dialog.FileName;
                }

                savepathService.SetSavePath(nonDuplicatefilePath);
            }
        }


        private void BuildLinq()
        {
            IRequiringBuildLinq BuildLinqService = (IRequiringBuildLinq)_databaseAction;

            // Getting linq shards from Datafilters view
            List<string> shardsCollection = _model.GetLinqShardsRequest.Invoke();

            if (shardsCollection != null)
            {
                List<string> ContextPropertyNames = new List<string>()
                             {   nameof(Person.Date),
                                 nameof(Person.FirstName),
                                 nameof(Person.SurName),
                                 nameof(Person.LastName),
                                 nameof(Person.City),
                                 nameof(Person.Country)
                             };

                int inc = 0;

                BuildLinqService.LinqExpression = "";

                foreach (string shard in shardsCollection)
                {
                    if (shard.Trim() != "")
                    {
                        if (BuildLinqService.LinqExpression.Length != 0)
                        {
                            BuildLinqService.LinqExpression += " && ";
                        }
                        BuildLinqService.LinqExpression +=
                            ContextPropertyNames[inc] + "== \"" + shard.Trim() + "\"";
                    }

                    inc++;
                }
            }
        }

    }
}
