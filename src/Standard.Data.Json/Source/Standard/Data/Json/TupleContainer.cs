using System;

namespace Standard.Data.Json
{
	internal sealed class TupleContainer
	{
		private int _size;
		private int _index;

		private object _1, _2, _3, _4, _5, _6, _7, _8;

		public TupleContainer(int size)
		{
			_size = size;
		}


		public Tuple<T1> ToTuple<T1>()
		{
			return new Tuple<T1>((T1)_1);
		}

		public Tuple<T1, T2> ToTuple<T1, T2>()
		{
			return new Tuple<T1, T2>((T1)_1, (T2)_2);
		}

		public Tuple<T1, T2, T3> ToTuple<T1, T2, T3>()
		{
			return new Tuple<T1, T2, T3>((T1)_1, (T2)_2, (T3)_3);
		}

		public Tuple<T1, T2, T3, T4> ToTuple<T1, T2, T3, T4>()
		{
			return new Tuple<T1, T2, T3, T4>((T1)_1, (T2)_2, (T3)_3, (T4)_4);
		}

		public Tuple<T1, T2, T3, T4, T5> ToTuple<T1, T2, T3, T4, T5>()
		{
			return new Tuple<T1, T2, T3, T4, T5>((T1)_1, (T2)_2, (T3)_3, (T4)_4, (T5)_5);
		}

		public Tuple<T1, T2, T3, T4, T5, T6> ToTuple<T1, T2, T3, T4, T5, T6>()
		{
			return new Tuple<T1, T2, T3, T4, T5, T6>((T1)_1, (T2)_2, (T3)_3, (T4)_4, (T5)_5, (T6)_6);
		}

		public Tuple<T1, T2, T3, T4, T5, T6, T7> ToTuple<T1, T2, T3, T4, T5, T6, T7>()
		{
			return new Tuple<T1, T2, T3, T4, T5, T6, T7>((T1)_1, (T2)_2, (T3)_3, (T4)_4, (T5)_5, (T6)_6, (T7)_7);
		}

		public Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> ToTuple<T1, T2, T3, T4, T5, T6, T7, TRest>()
		{
			return new Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>((T1)_1, (T2)_2, (T3)_3, (T4)_4, (T5)_5, (T6)_6, (T7)_7, (TRest)_8);
		}

		public void Add(object value)
		{
			switch (_index)
			{
				case 0:
					_1 = value;
					break;
				case 1:
					_2 = value;
					break;
				case 2:
					_3 = value;
					break;
				case 3:
					_4 = value;
					break;
				case 4:
					_5 = value;
					break;
				case 5:
					_6 = value;
					break;
				case 6:
					_7 = value;
					break;
				case 7:
					_8 = value;
					break;
			}
			_index++;
		}
	}
}
