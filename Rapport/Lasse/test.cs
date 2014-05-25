private static IEnumerable<Tuple<double, double>> CreateListOfTuples(Cons inputCons)
{
	foreach (var obj in inputCons)
	{
		var pair = (Cons)obj;

		//Unboxing must normally use the exact type.
		//We use dynamic as we don't know the exact type of the boxed value.  
		var x = (dynamic)pair.car;
		var y = (dynamic)pair.cdr;

		//x and y must be castable to double for this to work
		yield return new Tuple<double, double>((double)x, (double)y);
	}
}