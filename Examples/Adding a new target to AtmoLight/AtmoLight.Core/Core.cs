using ...;

namespace AtmoLight
{
	...
	public enum Target
	{
		...,
		...,
		...,
		Example
	}
	...

	public class Core
	{
		#region Fields
		...
		...
		// Example Settings Fields
		public string exampleIP;
		public int examplePort;
		#endregion
		
		...
		
		#region Configuration Methods (set)
		...
		public void AddTarget(Target target)
		{
			...
		    else if (target == Target.Example)
			{
				targets.Add(new ExampleHandler());
			}
		}
		...
	}
}