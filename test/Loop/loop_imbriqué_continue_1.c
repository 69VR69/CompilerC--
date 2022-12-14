int main() {
	int a;
	a = 0;

	while (a < 10)
	{
		a = 1 + a;
		int b;
		b = 2;
		if (a == 8)
			continue;
		while (b < 10)
		{
			if (b == 5)
				continue;
			b = 1 + b;
			int c;
			c = 1 + (2 * b) + a;
		}

		if (a == 5)
			continue;
		b = 1 + (2 * a);
	}
}
