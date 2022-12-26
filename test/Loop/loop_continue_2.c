int main() {
	int a;
	a = 0;

	do
	{
		a = 1 + a;
		int b;
		b = 1 + (2 * a);
		if (a == 2)
			continue;
	} while (a < 3);
}
