
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 字符串表达式
/// </summary>
public class StringExpression
{
		#region 中缀转后缀
		/// <summary>
		/// 中缀表达式转换为后缀表达式
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static string InfixToPostfix(string expression) 
		{
			Stack<char> operators = new Stack<char>();
			StringBuilder result = new StringBuilder();
			for (int i = 0; i < expression.Length; i++) {
				char ch = expression[i];
				if (char.IsWhiteSpace(ch)) continue;
				switch (ch) {
				case '+': 
				case '-':
					while (operators.Count > 0) {
						char c = operators.Pop();   //pop Operator
						if (c == '(') {
							operators.Push(c);      //push Operator
							break;
						}
						else {
							result.Append(c);
						}
					}
					operators.Push(ch);
					result.Append(" ");
					break;
				case '*': 
				case '/':
					while (operators.Count > 0) {
						char c = operators.Pop();
						if (c == '(') {
							operators.Push(c);
							break;
						}
						else {
							if (c == '+' || c == '-') {
								operators.Push(c);
								break;
							}
							else {
								result.Append(c);
							}
						}
					}
					operators.Push(ch);
					result.Append(" ");
					break;
				case '(':
					operators.Push(ch);
					break;
				case ')':
					while (operators.Count > 0) {
						char c = operators.Pop();
						if (c == '(') {
							break;
						}
						else {
							result.Append(c);
						}
					}
					break;
				default:
					result.Append(ch);
					break;
				}
			}
			while (operators.Count > 0){
				result.Append(operators.Pop()); //pop All Operator
			}
			return result.ToString();
		}
		
		#endregion
		/// <summary>
		/// 求值的经典算法
		/// </summary>
		/// <param name="expression">字符串表达式</param>
		/// <returns></returns>
		public static double Parse(string expression)
		{
			string postfixExpression = InfixToPostfix(expression);
			Stack<double> results = new Stack<double>();
			double x, y;
			for (int i = 0; i < postfixExpression.Length; i++)
			{
				char ch = postfixExpression[i];
				if (char.IsWhiteSpace(ch)) continue;
				switch (ch)
				{
				case '+':
					y = results.Pop();
					x = results.Pop();
					results.Push(x + y);
					break;
				case '-':
					y = results.Pop();
					x = results.Pop();
					results.Push(x - y);
					break;
				case '*':
					y = results.Pop();
					x = results.Pop();
					results.Push(x * y);
					break;
				case '/':
					y = results.Pop();
					x = results.Pop();
					results.Push(x / y);
					break;
				default:
					int pos = i;
					StringBuilder operand = new StringBuilder();
					do
					{
						operand.Append(postfixExpression[pos]);
						pos++;
					} while (char.IsDigit(postfixExpression[pos]) || postfixExpression[pos] == '.');
					i = --pos;
					results.Push(double.Parse(operand.ToString()));
					break;
				}
			}
			return results.Peek();
		}
}
