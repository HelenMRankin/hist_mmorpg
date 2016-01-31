using Microsoft.Pex.Framework.Suppression;
// <copyright file="PexAssemblyInfo.cs">Copyright ©  2015</copyright>
using Microsoft.Pex.Framework.Coverage;
using Microsoft.Pex.Framework.Creatable;
using Microsoft.Pex.Framework.Instrumentation;
using Microsoft.Pex.Framework.Settings;
using Microsoft.Pex.Framework.Validation;

// Microsoft.Pex.Framework.Settings
[assembly: PexAssemblySettings(TestFramework = "VisualStudioUnitTest")]

// Microsoft.Pex.Framework.Instrumentation
[assembly: PexAssemblyUnderTest("hist_mmorpg")]
[assembly: PexInstrumentAssembly("QuickGraph.Serialization")]
[assembly: PexInstrumentAssembly("System.Core")]
[assembly: PexInstrumentAssembly("System.Windows.Forms")]
[assembly: PexInstrumentAssembly("System.Configuration")]
[assembly: PexInstrumentAssembly("protobuf-net")]
[assembly: PexInstrumentAssembly("Lidgren.Network")]
[assembly: PexInstrumentAssembly("RiakClient")]
[assembly: PexInstrumentAssembly("QuickGraph")]

// Microsoft.Pex.Framework.Creatable
[assembly: PexCreatableFactoryForDelegates]

// Microsoft.Pex.Framework.Validation
[assembly: PexAllowedContractRequiresFailureAtTypeUnderTestSurface]
[assembly: PexAllowedXmlDocumentedException]

// Microsoft.Pex.Framework.Coverage
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "QuickGraph.Serialization")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Core")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Windows.Forms")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Configuration")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "protobuf-net")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "Lidgren.Network")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "RiakClient")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "QuickGraph")]
[assembly: PexSuppressExplorableEvent("hist_mmorpg.TestSuite")]

