int test(int a, int b)
{
	int c;
	c = a / b;
}

int main() {
	int a;
	a = 5;
	a = test(a, 10)+1;
}