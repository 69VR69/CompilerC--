int main() {
	int a;
	
	for (a = 0; a < 3; a = a + 1) {
		int b;
		if (a > 2)
			break;
		for (b = 0; b < 3; b = b + 1) {
			int c;
			c = b + 8 * a;
			if(b==2)
				break;
		}
		if (a == 2)
			break;
		b = a + 1;
	}
}
