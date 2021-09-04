﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLedger.Extensibility.Export
{
    /// <summary>
    /// This is a wrapper for exported List objects. It keeps a reference to an original list, mimics basic methods but hides interfaces 
    /// that might identify the origin as a list. Replacing lists with the wrapper prevents impicit conversion by connectors that causes loosing 
    /// a reference to the origin. So, wrapped lists properly supply mutability (manipulations with the exported list are reflected in the origin).
    /// </summary>
    public class ListAdapter<T>
    {
        public ListAdapter(IList<T> origin = null) => Origin = origin ?? new List<T>();

        public IList<T> Origin { get; }

        public T this[int index]
        {
            get => Origin[index];
            set => Origin[index] = value;
        }

        public int Count => Origin.Count;
        public void RemoveAt(int index) => Origin.RemoveAt(index);
        public void Insert(int index, T value) => Origin.Insert(index, value);
        public void Add(T value) => Origin.Add(value);
        public override string ToString() => Origin.ToString();
    }

    public static class ListAdapter
    {
        public static ListAdapter<Tuple<Values.Value, bool>> GetPostXDataSortValues(PostXData postXData) => new ListAdapter<Tuple<Values.Value, bool>>(postXData?.SortValues);
        public static ListAdapter<Tuple<Values.Value, bool>> GetAccountXDataSortValues(Accounts.AccountXData accountXData) => new ListAdapter<Tuple<Values.Value, bool>>(accountXData?.SortValues);
        public static ListAdapter<Values.Value> GetValueSequence(Values.Value value) => new ListAdapter<Values.Value>(value?.AsSequence);
        public static void SetValueSequence(Values.Value value, ListAdapter<Values.Value> sequence) => value?.SetSequence(sequence?.Origin);
        public static Values.Value CreateValue(ListAdapter<Values.Value> sequence) => new Values.Value(sequence.Origin);
        public static ListAdapter<Post> GetPosts(Xacts.XactBase xact) => new ListAdapter<Post>(xact?.Posts);
        public static ListAdapter<Post> GetPosts(Accounts.AccountXData xdata) => new ListAdapter<Post>(xdata?.ReportedPosts);
    }
}
