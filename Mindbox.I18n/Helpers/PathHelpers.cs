namespace Mindbox.I18n;

public static class PathHelpers
{
	public static string ConvertToUnixPath(string path)
	{
		// Windows works with both \ and / as path separator yet unix systems only work with / so we always use the latter.
		return path.Replace('\\', '/');
	}
}