int test(int a, int b)
{
	return a + b;
}

int main() {
	int a;
	a = 5;
	a = 2*test(a, 10)+1;
}