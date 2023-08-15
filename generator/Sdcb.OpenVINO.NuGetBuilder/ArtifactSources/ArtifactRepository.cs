namespace Sdcb.OpenVINO.NuGetBuilder.ArtifactSources;

public class ArtifactRepository
{
    private readonly OpenVINOFileTreeRoot _source;

    public ArtifactRepository(OpenVINOFileTreeRoot source)
    {
        _source = source;
    }
}
