int main() {
	int a;
	a = 0;

	while (a < 10)
	{
		a = 1 + a;
		int b;
		b = 2;
		if (a > 8)
			break;
		while (b < 10)
		{
			b = 1 + b;
			int c;
			c = 1 + (2 * b) + a;
			if (b == 5)
				break;
		}
		if (a == 5)
			break;

		b = 1 + (2 * a);
	}
}
