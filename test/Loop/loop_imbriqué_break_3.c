int main() {
	int a;
	
	for (a = 0; a < 2; a = a + 1) {
		int b;
		if (a > 1)
			break;
		for (b = 0; b < 2; b = b + 1) {
			int c;
			c = b + 8 * a;
			if(b==1)
				break;
		}
		if (a == 1)
			break;
		b = a + 1;
	}
}
