﻿using System;
using System.Collections.Generic;

namespace FLS.ServerSide.SharingObject
{
    public abstract class PagedListBase
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
        public int FirstRow
        {
            get { return (CurrentPage - 1) * PageSize + 1; }
        }
        public int LastRow
        {
            get { return Math.Min(CurrentPage * PageSize, TotalItems); }
        }
    }
    public class PagedList<T> : PagedListBase where T: class
    {
        public List<T> Items { get; set; }
        public PagedList(){
            CurrentPage = 1;
            PageSize = 10;
            TotalPage = 1;
            TotalItems = 0;
            Items = null;
        }
        //public PagedList(int _current, int _pageSize, int _totalPage, int _totalItem, List<T> _items)
        //{
        //    if(_current < 1 || _current > _totalPage)
        //        throw new ArgumentOutOfRangeException("_current out of range");
        //    if(_pageSize < 1 || _pageSize > 200)
        //        throw new ArgumentOutOfRangeException("_pageSize out of range");
        //    if(_totalPage < 1)
        //        throw new ArgumentOutOfRangeException("_totalPage out of range");
        //    if(_totalItem < 0)
        //        throw new ArgumentOutOfRangeException("_totalItem out of range");
        //    if(_items.Count > _pageSize)
        //        throw new ArgumentOutOfRangeException("_items in page larger than _pageSize");

        //    CurrentPage = _current;
        //    PageSize = _pageSize;
        //    TotalPage = _totalPage;
        //    TotalItem = _totalItem;
        //    Items = _items;
        //}
    }
    public class ApiResponse<T>
    {
        public bool HasError { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public T Data { get; set; }
        public ApiResponse(T _data)
        {
            Data = _data;
        }
    }
    public class IdNameModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }
}