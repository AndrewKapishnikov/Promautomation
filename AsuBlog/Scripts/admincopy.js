$(function () {

  var JustBlog = {};
  
    JustBlog.GridManager = {};
   
    

  //*************************POSTS GRID***************************************************
    JustBlog.GridManager.postsGrid = function (gridName, pagerName)
    {

        //*** Event handlers
        var afterclickPgButtons = function (whichbutton, formid, rowid) {
          tinyMCE.get("ShortDescription").setContent(formid[0]["ShortDescription"].value);
          tinyMCE.get("Description").setContent(formid[0]["Description"].value);
          };

          //Our plan is when the dialog is shown we have to convert the textareas into
          //editors and when the dialog is closed we have to do the reverse thing.
        var afterShowForm = function (form) {
          tinyMCE.execCommand('mceAddControl', false, "ShortDescription");
          tinyMCE.execCommand('mceAddControl', false, "Description");
        };

        var onClose = function (form) {
          tinyMCE.execCommand('mceRemoveControl', false, "ShortDescription");
          tinyMCE.execCommand('mceRemoveControl', false, "Description");
        };

        var beforeSubmitHandler = function (postdata, form) {
          var selRowData = $(gridName).getRowData($(gridName).getGridParam('selrow')); //Никак не пойму...
          if (selRowData["PostedOn"])
            postdata.PostedOn = selRowData["PostedOn"];
          postdata.ShortDescription = tinyMCE.get("ShortDescription").getContent();
          postdata.Description = tinyMCE.get("Description").getContent();

          return [true];
        };

        var colNames = [  //количество элементов массиве должно быть равно количеству элементов в columns
			    'Id',
			    'Title',
			    'Short Description',
			    'Description',
			    'Category',
			    'Category',
			    'Tags',
			    'Meta',
			    'Url Slug',
			    'Published',
			    'Posted On',
			    'Modified'
        ];

        var columns = [];

        columns.push({
          name: 'Id',
          index: 'Id',
          hidden: false,
          width: 50,
          key: true
        });

        columns.push({
          name: 'Title',
          index: 'Title',
          width: 250,
          editable: true,
          editoptions: {
            size: 43,
            maxlength: 500
          },
          editrules: {
            required: true
          },
          formatter: 'showlink',
          formatoptions: {
            target: "_new",
            baseLinkUrl: '/Admin/GoToPost' //при клике на ссылку открывается окно этой статьи
          }
        });

        columns.push({
          name: 'ShortDescription',
          index: 'ShortDescription',
          width: 250,
          editable: true,
          sortable: false,
          hidden: true,
          edittype: 'textarea',
          editoptions: {
            rows: "10",
            cols: "100"
          },
          editrules: {
            custom: true,
            custom_func: function (val, colname) {                     //Это валидация наличия текста в поле
              val = tinyMCE.get("ShortDescription").getContent();
              if (val) return [true, ""];
              return [false, colname + ": Field is required"];
            },
            edithidden: true
          }
        });

        columns.push({
          name: 'Description',
          index: 'Description',
          width: 250,
          editable: true,
          sortable: false,
          hidden: true,
          edittype: 'textarea',
          editoptions: {
            rows: "40",
            cols: "100"
          },
          editrules: {
            custom: true,
            custom_func: function (val, colname) {
              val = tinyMCE.get("Description").getContent();
              if (val) return [true, ""];
              return [false, colname + ": Field is requred"];
            },
            edithidden: true
          }
        });

        columns.push({
          name: 'Category.Id',
          hidden: true,
          editable: true,
          edittype: 'select',
          editoptions: {
            style: 'width:250px;',
            dataUrl: '/Admin/GetCategoriesHtml'
        
          },
          editrules: {
            required: true,
            edithidden: true
          }
        });

        columns.push({
        
          name: 'Category.Name',
          index: 'Category',    //по индексу  отправляется тип сортировки sidx
          width: 150
        });

        columns.push({
          name: 'Tags',
          width: 150,
          editable: true,
          edittype: 'select',
          editoptions: {
            style: 'width:250px;',
            dataUrl: '/Admin/GetTagsHtml',
            multiple: true
          },
          editrules: {
            required: true
          }
        });

        columns.push({
          name: 'Meta',
          width: 100,
          sortable: false,
          editable: true,
          edittype: 'textarea',
          editoptions: {
            rows: "2",
            cols: "40",
            maxlength: 1000
          },
          editrules: {
            required: true
          }
        });

        columns.push({
          name: 'UrlSlug',
          width: 200,
          sortable: false,
          editable: true,
          editoptions: {
            size: 43,
            maxlength: 200
          },
          editrules: {
            required: true
          }
        });

        columns.push({
          name: 'Published',
          index: 'Published',
          width: 100,
          align: 'center',
          editable: true,
          edittype: 'checkbox',
          editoptions: {
            value: "true:false",
            defaultValue: 'false'
          }
        });

        columns.push({
          name: 'PostedOn',
          index: 'PostedOn',         //sidx параметр, отправляемый на сервер Cортировка
          width: 150,
          align: 'center',
          sorttype: 'date',
          datefmt: 'm/d/Y'
        });

        columns.push({
          name: 'Modified',
          index: 'Modified',
          width: 100,
          align: 'center',
          sorttype: 'date',
          datefmt: 'm/d/Y'
        });
    //----------------------------------------------
        $(gridName).jqGrid({
          url: '/Admin/Posts',
          datatype: 'json',
          mtype: 'GET',
          height: 'auto',
          toppager: true,  //наличия вкладки Pager сверху таблицы

          colNames: colNames,
          colModel: columns,

          pager: pagerName,
          rownumbers: true,
          rownumWidth: 40,
          rowNum: 5,        //количество отображаемых строк   //rows
          rowList: [10, 20, 30],

          sortname: 'PostedOn',                              
          sortorder: 'desc',                                  //sord       
          viewrecords: true,

          jsonReader: {
            repeatitems: false
          },

            afterInsertRow: function (rowid, rowdata, rowelem) {   //срабатывает каждый раз, когда строка успешно вставлена в сетку
            var published = rowdata["Published"];

            if (!published) {
              $(gridName).setRowData(rowid, [], {
                color: '#9D9687'
              });
              $(gridName + " tr#" + rowid + " a").css({
                color: '#9D9687'
              });
            }

            var tags = rowdata["Tags"];
            var tagStr = "";

            $.each(tags, function (i, t) { 
                if (tagStr) tagStr += ", ";
              tagStr += t.Name;
                });

                //$(function () {   //Пример
                //    var myArray = ["один", "два", "три", "четыре", "пять"];
                //    $.each(myArray, function (index, value) {
                //        console.log("INDEX: " + index + " VALUE: " + value);
                //    });
                //});


            $(gridName).setRowData(rowid, { "Tags": tagStr});
          }
        });
    //----------------------------------------------
        // configuring add options
        var addOptions = {
          url: '/Admin/AddPost',
          addCaption: 'Add Post',
          processData: "Saving...",
          width: 900,
          closeAfterAdd: true,
          closeOnEscape: true,
          afterclickPgButtons: afterclickPgButtons,
          afterShowForm: afterShowForm,
          onClose: onClose,
          afterSubmit: JustBlog.GridManager.afterSubmitHandler,
          beforeSubmit: beforeSubmitHandler
        };

        var editOptions = {
          url: '/Admin/EditPost',
          editCaption: 'Edit Post',
          processData: "Saving...",
          width: 900,
          closeAfterEdit: true,
          closeOnEscape: true,
          afterclickPgButtons: afterclickPgButtons,
          afterShowForm: afterShowForm,
          onClose: onClose,
          afterSubmit: JustBlog.GridManager.afterSubmitHandler,
          beforeSubmit: beforeSubmitHandler
        };

        var deleteOptions = {
          url: '/Admin/DeletePost',
          caption: 'Delete Post',
          processData: "Saving...",
          msg: "Delete the Post?",
          closeOnEscape: true,
          afterSubmit: JustBlog.GridManager.afterSubmitHandler
        };

          $(gridName).navGrid(pagerName,
              {
                  cloneToTop: true, //заставляет навигатор появляться как в верхней, так и в нижней части сетки. 
                  search: true,
                  view:true
              },
              editOptions,
              addOptions,
              deleteOptions);
    };


 //*************************************** CATEGORIES GRID****************************************
  JustBlog.GridManager.categoriesGrid = function (gridName, pagerName) {
    var colNames = ['Id', 'Name', 'Url Slug', 'Description'];

    var columns = [];

    columns.push({
      name: 'Id',
      index: 'Id',
      hidden: true,
      sorttype: 'int',
      key: true,
      editable: false,
      editoptions: {
        readonly: true
      }
    });

    columns.push({
      name: 'Name',
      index: 'Name',
      width: 200,
      editable: true,
      edittype: 'text',
      editoptions: {
        size: 30,
        maxlength: 50
      },
      editrules: {
        required: true
      }
    });

    columns.push({
      name: 'UrlSlug',
      index: 'UrlSlug',
      width: 200,
      editable: true,
      edittype: 'text',
      sortable: false,
      editoptions: {
        size: 30,
        maxlength: 50
      },
      editrules: {
        required: true
      }
    });

    columns.push({
      name: 'Description',
      index: 'Description',
      width: 200,
      editable: true,
      edittype: 'textarea',
      sortable: false,
      editoptions: {
        rows: "4",
        cols: "28"
      }
    });

    $(gridName).jqGrid({
      url: '/Admin/Categories',
      datatype: 'json',
      mtype: 'GET',
      height: 'auto',
      toppager: true,
      colNames: colNames,
      colModel: columns,
      pager: pagerName,
      rownumbers: true,
      rownumWidth: 40,
      rowNum: 500,
      sortname: 'Name',
      loadonce: true,
      jsonReader: {
        repeatitems: false
      }
    });

    var editOptions = {
      url: '/Admin/EditCategory',
      width: 400,
      editCaption: 'Edit Category',
      processData: "Saving...",
      closeAfterEdit: true,
      closeOnEscape: true,
      afterSubmit: function (response, postdata) {
        var json = $.parseJSON(response.responseText);

        if (json) {
          $(gridName).jqGrid('setGridParam', { datatype: 'json' });
          return [json.success, json.message, json.id];
        }

        return [false, "Failed to get result from server.", null];
      }
    };

    var addOptions = {
      url: '/Admin/AddCategory',
      width: 400,
      addCaption: 'Add Category',
      processData: "Saving...",
      closeAfterAdd: true,
      closeOnEscape: true,
      afterSubmit: function (response, postdata) {
        var json = $.parseJSON(response.responseText);

        if (json) {
          $(gridName).jqGrid('setGridParam', { datatype: 'json' });
          return [json.success, json.message, json.id];
        }

        return [false, "Failed to get result from server.", null];
      }
    };

    var deleteOptions = {
      url: '/Admin/DeleteCategory',
      caption: 'Delete Category',
      processData: "Saving...",
      width: 500,
      msg: "Delete the category? This will delete all the posts belonged to this category as well.",
      closeOnEscape: true,
      afterSubmit: JustBlog.GridManager.afterSubmitHandler
    };

    // configuring the navigation toolbar.
    $(gridName).jqGrid('navGrid', pagerName, {
      cloneToTop: true,
        search: false,
       },

        editOptions, addOptions, deleteOptions);
  };

  //**************************************** TAGS GRID **********************************************
  JustBlog.GridManager.tagsGrid = function (gridName, pagerName) {
    var colNames = ['Id', 'Name', 'Url Slug', 'Description'];

    var columns = [];

    columns.push({
      name: 'Id',
      index: 'Id',
      //hidden: false,
     // sorttype: 'int',
      key: true,
      editable: false,
      editoptions: {
        readonly: true
      }
    });

    columns.push({
      name: 'Name',
      index: 'Name',
      width: 200,
      editable: true,
      edittype: 'text',
      editoptions: {
        size: 30,
        maxlength: 50
      },
      editrules: {
        required: true
      }
    });

    columns.push({
      name: 'UrlSlug',
      index: 'UrlSlug',
      width: 200,
      editable: true,
      edittype: 'text',
      sortable: false,
      editoptions: {
        size: 30,
        maxlength: 50
      },
      editrules: {
        required: true
      }
    });

    columns.push({
      name: 'Description',
      index: 'Description',
      width: 200,
      editable: true,
      edittype: 'textarea',
      sortable: false,
      editoptions: {
        rows: "4",
        cols: "28"
      }
    });

    $(gridName).jqGrid({
      url: '/Admin/Tags',
      datatype: 'json',
      mtype: 'GET',
      height: 'auto',
      toppager: true,
      colNames: colNames,
      colModel: columns,
      pager: pagerName,
      rownumbers: true,
      rownumWidth: 40,
      rowNum: 500,
      sortname: 'Name',
      loadonce: true,   //Не отправляет запрос при изменении например порядка сортировки по столбцам
      jsonReader: {
        repeatitems: false
      }
    });

    var editOptions = {
      url: '/Admin/EditTag',
      editCaption: 'Edit Tag',
      processData: "Saving...",
      closeAfterEdit: true,
      closeOnEscape: true,
      width: 400,
      afterSubmit: function (response, postdata) {
        var json = $.parseJSON(response.responseText);

        if (json) {
          $(gridName).jqGrid('setGridParam', { datatype: 'json' });
          return [json.success, json.message, json.id];
        }

        return [false, "Failed to get result from server.", null];
      }
    };

    var addOptions = {
      url: '/Admin/AddTag',
      addCaption: 'Add Tag',
      processData: "Saving...",
      closeAfterAdd: false,
      closeOnEscape: true,
      width: 400,
      afterSubmit: function (response, postdata) {
        var json = $.parseJSON(response.responseText);

        if (json) {
          $(gridName).jqGrid('setGridParam', { datatype: 'json' });
          return [json.success, json.message, json.id];
        }

        return [false, "Failed to get result from server.", null];
      }
    };

    var deleteOptions = {
      url: '/Admin/DeleteTag',
      caption: 'Delete Tag',
      processData: "Saving...",
      width: 500,
      msg: "Delete the tag? This will delete all the posts belonged to this tag as well.",
      closeOnEscape: true,
      afterSubmit: JustBlog.GridManager.afterSubmitHandler
    };

    // configuring the navigation toolbar.
    $(gridName).jqGrid('navGrid', pagerName, {
      cloneToTop: true,
      search: false
    },

    editOptions, addOptions, deleteOptions);
    };

//******************************TAGS GRID END *********************************************


  //var a = JustBlog.GridManager.afterSubmitHandler(); // так можно
  JustBlog.GridManager.afterSubmitHandler = function (response, postdata) {

    var json = $.parseJSON(response.responseText);

    if (json) return [json.success, json.message, json.id];

    return [false, "Failed to get result from server.", null];
    };



  $("#tabs").tabs(  {                 //Создаёт вкладки
    show: function (event, ui) {

      if (!ui.tab.isLoaded) {

        var gdMgr = JustBlog.GridManager,
				fn, gridName, pagerName;

        switch (ui.index) {
          case 0:
            fn = gdMgr.postsGrid;
            gridName = "#tablePosts";
            pagerName = "#pagerPosts";
            break;
          case 1:
            fn = gdMgr.categoriesGrid;
            gridName = "#tableCats";
            pagerName = "#pagerCats";
            break;
          case 2:
            fn = gdMgr.tagsGrid;
            gridName = "#tableTags";
            pagerName = "#pagerTags";
            break;
        };

        fn(gridName, pagerName);
        ui.tab.isLoaded = true;
      }

    }
    }  );

    //var a = JustBlog.GridManager.tagsGrid;
    
});




























