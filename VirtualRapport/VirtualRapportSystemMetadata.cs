using System;
using System.Composition;
using Microsoft.Psi;

namespace OpenSense.Components.VirtualRapport {

    [Serializable]
    public class VirtualRapportSystemComponentConfiguration : ConventionalComponentConfiguration {

        //You can put options here, they will be serialized and deserialized. A method SetProperty() is provided for triggering IPropertyChanged event if necessary.

        public override IComponentMetadata GetMetadata() => new VirtualRapportSystemComponentMetadata();

        protected override object Instantiate(Pipeline pipeline, IServiceProvider serviceProvider) => new VirtualRapportSystem(pipeline) {
            //Logger = (serviceProvider?.GetService(typeof(ILoggerFactory)) as ILoggerFactory)?.CreateLogger(Name), //if you want an ILogger, then define a field Logger for /psi component.
        };
    }

    [Export(typeof(IComponentMetadata))] //This ExportAttribute is from System.Composition namespace, not System.ComponentModel.Composition
    public class VirtualRapportSystemComponentMetadata : ConventionalComponentMetadata {

        public override string Description => "Virtual Rapport System";

        protected override Type ComponentType => typeof(VirtualRapportSystem); //Unspecified generic types are not supported, set all generic parameters here.

        public override ComponentConfiguration CreateConfiguration() => new VirtualRapportSystemComponentConfiguration();
    }
}
