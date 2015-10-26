using UnityEngine;
using System.Collections;
using System ;

namespace VectorList{
	// </summary>  
	public class ListNode  
	{  
		public string data; //ElemType
		public ListNode(){}  // 构造函数  
		public ListNode next;  
	}  	
	/// <summary>  
	/// 链表类  
	/// </summary>  
	public class LinkList{  
		
		private ListNode first;   //第一个结点  
		public LinkList(){  
			first = null;  
		}  
		
		public bool IsEmpty()  
		{  
			return first == null;  
		}  
		/// <summary>  
		/// 在第k个元素之后插入x  
		/// </summary>  
		/// <param name="k"></param>  
		/// <param name="x"></param>  
		/// <returns></returns>  
		public LinkList Insert( int k, string x )  
		{  
			if( k<0 )  
				return null;  
			ListNode pNode = first;  //pNode将最终指向第k个结点  
			for( int index = 1; index<k && pNode != null; index++ )  
				pNode = pNode.next;  
			if( k>0 && pNode == null )  
				return null;//不存在第k个元素  
			ListNode xNode = new ListNode();  
			xNode.data = x;  
			if(k>0)  
			{  
				//在pNode之后插入  
				xNode.next = pNode.next;  
				pNode.next = xNode;  
			}  
			else  
			{  
				//作为第一个元素插入  
				xNode.next = first;  
				first = xNode;  
			}  
			return this;  
		}  
		/// <summary>
		/// 获取位置队列的长度.
		/// </summary>
		public int Length()  
		{  
			ListNode current = first;  
			int length = 0;  
			while(current != null)  
			{  
				length++;  
				current = current.next;  
			}  
			return length;  
		}  
		
		/// <summary>  
		/// 返回第k个元素至x中  
		/// </summary>  
		/// <param name="k"></param>  
		/// <param name="x"></param>  
		/// <returns>如果不存在第k个元素则返回false,否则返回true</returns>  
		public bool Find( int k, ref string x )  
		{  
			if( k<1 )  
				return false;  
			ListNode current = first;  
			int index = 1;
			while( index<k && current != null )  
			{  
				current = current.next;  
				index++;  
			}  
			if( current != null )  
			{  
				x = current.data;  
				return true;  
			}  
			return false;  
		}  
		/// <summary>
		/// 判断位置队列是否存在x，并得到其在队列中的序号.
		/// </summary>
		/// <returns>The index.</returns>
		/// <param name="x">The x coordinate.</param>
		public int get_Index(string x)
		{
			ListNode current = first;  
			int index = 1;  
			while(current != null )  
			{  
				if(current.data.Equals(x))
					return index;
				else{
					current = current.next;  
					index++; 
				} 
			}
			return 0;
		}
		/// <summary>
		/// 获取位置队列在k处的数据.
		/// </summary>
		/// <returns>The listnode.</returns>
		/// <param name="k">K.</param>
		public string get_Listnode(int k)
	   {
//			if (k < 1) 
//				return;
			ListNode current = first;
			string x = "";
			int index = 1;
			while( index<k && current != null )  
			{  
				current = current.next;  
				index++;  
			}  
			if( current != null )  
			{  
				x = current.data;  
			}
			return x;
//			if (current == null)
//				return;
		}
		/// <summary>  
		/// 返回x所在的位置  
		/// </summary>  
		/// <param name="x"></param>  
		/// <returns>如果x不在表中则返回0</returns>  
		public int Search( string x )  
		{  
			ListNode current = first;  
			int index = 1;  
			while( current != null && current.data !=x )  
			{  
				current = current.next;  
				index++;  
			}  
			if(current != null)  
				return index;  
			return 0;  
		}  
		/// <summary>  
		/// 删除第k个元素，并用x返回其值  
		/// </summary>  
		/// <param name="k"></param>  
		/// <param name="x"></param>  
		/// <returns></returns>  
		public LinkList Delete( int k)  
		{  
			//如果不存在第k个元素则引发异常  
			if( k<1 || first == null )  
				return null;  
			ListNode pNode = first;  //pNode将最终指向第k个结点  
			//将pNode移动至第k个元素，并从链表中删除该元素  
			if( k == 1 ) //pNode已经指向第k个元素  
				first = first.next;  //删除之  
			else  
			{  
				//用qNode指向第k-1个元素  
				ListNode qNode = first;  
				for( int index=1; index< k-1 && qNode != null; index++ )  
					qNode = qNode.next;  
				if( qNode == null || qNode.next == null )  
					return null;//不存在第k个元素  
				pNode = qNode.next; //pNode指向第k个元素  
				qNode.next = pNode.next; //从链表中删除第k个元素    
			}  
			return this;  
		}  
		//判断是否相同
		public bool Eques(LinkList x){
			if (this.Length() == x.Length()) {
				for (int index = 0; index < this.Length(); index++) {
					if (this.get_Listnode (index) != x.get_Listnode (index))
						return false;
				}
				return true;
			} else
				return false;
		}
		// 清空链表  
		public void Clear()  
		{  
			first = null;  
		}  	
	}  
}  


