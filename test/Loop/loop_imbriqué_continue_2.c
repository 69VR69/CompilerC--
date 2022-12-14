int main() {
	int a;
	a = 0;

	do
	{
		a = 1 + a;
		int b;
		b = 2;
		if (a == 8)
			continue;
		do {
			if(b==5)
				continue;
			b = 1 + b;
			int c;
			c = 2 * b + a;
		} while (b < 10);
		if (a == 5)
			continue;
		b = 1 + (2 * a);
	} while (a < 10);
}
