$(function () {

  var JustBlog = {};
     JustBlog.GridManager = {};
    

  //******************************************** POSTS GRID ***************************************************
  JustBlog.GridManager.postsGrid = function (gridName, pagerName) {

    //*** Event handlers
    var afterclickPgButtons = function (whichbutton, formid, rowid) {
      tinyMCE.get("ShortDescription").setContent(formid[0]["ShortDescription"].value);
      tinyMCE.get("Description").setContent(formid[0]["Description"].value);
      };

      // When the dialog is shown we have to convert the textareas into
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
      var selRowData = $(gridName).getRowData($(gridName).getGridParam('selrow')); 
      if (selRowData["PostedOn"])                                   //устанавливаем время для отправки из выбранной строки
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
            'Tags',
            'Category for Post',
            'Categorys',
            'Url Slug',
            'Meta',
            'Published',
            'Posted On',
            'Modified',
            'ImagePath',
            'NumberVisits',
            'Topic',
            'Subtopic',
            'Theme',
            'Subtheme',
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
      width: 300,
      editable: true,
      editoptions: {
        size: 80,
        maxlength: 500
      },
      editrules: {
        required: true
      },
      formatter: 'showlink',
      formatoptions: {
        target: "_new",
        baseLinkUrl: '/admin/ShowPost' //при клике на ссылку открывается окно этой статьи
      }
    });

    columns.push({
      name: 'ShortDescription',
      index: 'ShortDescription',
      width: 250,
      editable: true,
      sortable: false,
      hidden: true,   //прячем колонку
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
          name: 'Tags',
          width: 150,
          editable: true,
          edittype: 'select',
          editoptions: {
              style: 'width:300px;height:150px;',
              dataUrl: '/admin/GetTagsForPostsGrid',
              multiple: true
          },
          editrules: {
              required: true
          }
      });

      columns.push({
        name: 'Category for Post',
        hidden: true,
        editable: true,
        edittype: 'select',
        editoptions: {
          style: 'width:300px;',
           dataUrl: '/admin/GetCategoriesForPostsGrid',
           multiple: false

        },
        editrules: {
          required: true,
          edithidden: true
        }
      });

      columns.push({

          name: 'Categorys',
          index: 'Categorys',    //по индексу  отправляется тип сортировки sidx
          width: 350
      });
          

    columns.push({
        name: 'UrlSlug',
        width: 300,
        sortable: false,
        editable: true,
        editoptions: {
            size: 100,
            maxlength:900
        },
        editrules: {
            required: true
        }
      });

      columns.push({
          name: 'Meta',
          width: 300,
          sortable: false,
          editable: true,
          edittype: 'textarea',
          editoptions: {
              rows: "2",
              cols: "100",
              maxlength:900
          },
          editrules: {
              required: true
          }
      });

    columns.push({
        name: 'Published',
        index: 'Published',
        width: 30,
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
        datefmt: 'd/m/Y'
    });

    columns.push({
        name: 'Modified',
        index: 'Modified',
        width: 150,
        align: 'center',
        sorttype: 'date',
        datefmt: 'd/m/Y'
      });

    columns.push({
        name: 'ImagePath',
        width: 100,
        sortable: false,
   
      });

    columns.push({
        name: 'NumberVisits',
        width: 40,
        sortable: false,
        editable: true,
        editoptions: {
            size: 10,
            maxlength: 100
        },
        editrules: {
            required: true
        }
    });

    columns.push({
        name: 'Topic',
        width: 100,
        sortable: false,
        editable: true,
        editoptions: {
            size: 100,
            maxlength: 900
        },
        editrules: {
            required: true
        }
    });

    columns.push({
        name: 'Subtopic',
        width: 100,
        sortable: false,
        editable: true,
        editoptions: {
            size: 100,
            maxlength: 900
        },
        editrules: {
            required: false
        }
    });

    columns.push({
        name: 'Theme',
        width: 100,
        sortable: false,
        editable: true,
        editoptions: {
            size: 100,
            maxlength: 900
        },
        editrules: {
            required: false
        }
    });

    columns.push({
        name: 'Subtheme',
        width: 100,
        sortable: false,
        editable: true,
        editoptions: {
            size: 100,
            maxlength: 900
        },
        editrules: {
            required: false
        }
      });

//----------------------------------------------
      $(gridName).jqGrid({

       // server url and other ajax stuff
      url: '/admin/Posts',
      datatype: 'json',
      mtype: 'GET',
      height: 'auto',
      toppager: true,  //наличия вкладки Pager сверху таблицы

      colNames: colNames,
      colModel: columns,

       // pagination options
      pager: pagerName,
      rownumbers: true,
      rownumWidth: 40,
      rowNum: 30,        //количество отображаемых строк   //rows
      rowList: [10, 20, 30, 50, 100],

       // default sorting
      sortname: 'PostedOn',                              
      sortorder: 'desc',                                  //sord   

       // display the no. of records message
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
        $(gridName).setRowData(rowid, { "Tags": tagStr });
        var categorys = rowdata["Categorys"];
        var categorysStr = "";

        $.each(categorys, function (i, t) {
            if (categorysStr) categorysStr += ", ";
            categorysStr += t.Name;
        });

         $(gridName).setRowData(rowid, { "Categorys": categorysStr });
      }
    });
///--------------------------------------------------------------------------------
    // configuring add options
    var addOptions = {
      url: '/admin/AddPost',
      addCaption: 'Добавить Post',
      processData: "Сохранение...",
      width: 1000,
      closeAfterAdd: true,
      closeOnEscape: true,
      //после того как нажали кнопку Post
      afterclickPgButtons: afterclickPgButtons,
      afterShowForm: afterShowForm,
      onClose: onClose,
      //функция afterSubmit, вызываемая после получения ответа от сервера после отправки формы.Она выполняет обновление грида
      afterSubmit: JustBlog.GridManager.afterSubmitHandler, //когда пришёл ответ
      //перед отправкой сообщения на сервер
      beforeSubmit: beforeSubmitHandler
    };

    var editOptions = {
      url: '/admin/EditPost',
      editCaption: 'Редактировать Post',
      processData: "Сохранение...",
      width: 1000,
      closeAfterEdit: true,
      closeOnEscape: true,
      afterclickPgButtons: afterclickPgButtons,
      afterShowForm: afterShowForm,
      onClose: onClose,
      afterSubmit: JustBlog.GridManager.afterSubmitHandler,
      beforeSubmit: beforeSubmitHandler
    };

    var deleteOptions = {
      url: '/admin/DeletePost',
      caption: 'Удалить Post',
      processData: "Сохранение...",
      msg: "Удалить этот пост?",
      closeOnEscape: true,
      afterSubmit: JustBlog.GridManager.afterSubmitHandler
    };

      $(gridName).navGrid(pagerName,
          {
              cloneToTop: true, //заставляет навигатор появляться как в верхней, так и в нижней части сетки. 
              search: true,
              view:false
          },
          editOptions,
          addOptions,
          deleteOptions);
 };

 //***************************************  POSTS GRID  *********************************************


//***************************************CATEGORIES GRID********************************************
  JustBlog.GridManager.categoriesGrid = function (gridName, pagerName) {
     var colNames =
          ['Id', 'ParentId', 'Name', 'Url Slug','BoolArticle', 'Level','FullUrl', 'Description'];

    var columns = [];

    columns.push({
      name: 'Id',
      index: 'Id',
      width: 50,
      sorttype: 'int',
      key: true,

      });

    columns.push({
        name: 'ParentId',
        width: 200,
        sortable: false,
        editable: true,
        edittype: 'select',
        editoptions: {
            style: 'width:300px;',
            dataUrl: '/admin/GetCategoriesForParentCategoryGrid',
            multiple: false

        },
        editrules: {
            required: true,
            edithidden: true
        }
    });

    columns.push({
      name: 'Name',
      index: 'Name',
      width: 200,
      editable: true,
      edittype: 'text',
      sortable: false,
      editoptions: {
        size: 80,
        maxlength: 500
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
        size: 80,
        maxlength: 500
      },
      editrules: {
        required: true
      }
    });
      
    columns.push({
        name: 'BoolArticle',
        width: 50,
        sortable: false,
        editable: true,
        edittype: 'checkbox',
        editoptions: {
            value: "true:false",
            //defaultValue: 'false'
        }

    });
    columns.push({
        name: 'Level',
        width: 50,
        sortable: false
    });
    columns.push({
        name: 'FullUrl',
        width: 300,
        sortable: false,
        editable: true,
        edittype: 'text',
        sortable: false,
        editoptions: {
            size: 80,
            maxlength: 500
        },
        editrules: {
            required: false
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
        cols: "100",
        maxlength: 900
      }
    });

    $(gridName).jqGrid({
      url: '/admin/Categories',
      datatype: 'json',
      mtype: 'GET',
      height: 'auto',
      toppager: true,
      colNames: colNames,
      colModel: columns,
      pager: pagerName,
      rownumbers: true,
      rownumWidth: 40,
      rowNum: 20,
      rowList: [20, 30, 50, 100],
      sortname: 'Name',
      viewrecords: true,
      jsonReader: {
        repeatitems: false
      }

    
    });

    var editOptions = {
      url: '/admin/EditCategory',
      width: 700,
      editCaption: 'Редактировать выбранную Категорию',
      processData: "Сохранение...",
      closeAfterEdit: true,
      closeOnEscape: true,
      afterSubmit: function (response, postdata) {
        var json = $.parseJSON(response.responseText);

        if (json) {
          $(gridName).jqGrid('setGridParam', { datatype: 'json' });
          return [json.success, json.message, json.id];
        }

        return [false, "Ошибка при получении ответа с сервера", null];
      }
    };

    var addOptions = {
      url: '/admin/AddCategory',
      width: 700,
      addCaption: 'Добавить Категорию',
      processData: "Сохранение...",
      closeAfterAdd: true,
      closeOnEscape: true,
      afterSubmit: function (response, postdata) {
        var json = $.parseJSON(response.responseText);

        if (json) {
          $(gridName).jqGrid('setGridParam', { datatype: 'json' });
          return [json.success, json.message, json.id];
        }

          return [false, "Ошибка при получении ответа с сервера", null];
      }
    };

    var deleteOptions = {
      url: '/admin/DeleteCategory',
      caption: 'Удалить Категорию',
      processData: "Сохранение...",
      width: 500,
      msg: "Удалить Категорию?",
        closeOnEscape: false,
        closeOnDelete: false,
      afterSubmit: JustBlog.GridManager.afterSubmitHandler
    };

    // configuring the navigation toolbar.
    $(gridName).jqGrid('navGrid', pagerName, {
      cloneToTop: true,
        search: false,
        view: true
       },

        editOptions, addOptions, deleteOptions);
    };
  //***************************************  CATEGORIES GRID  **************************************


  //**************************************** TAGS GRID **********************************************
  JustBlog.GridManager.tagsGrid = function (gridName, pagerName) {
    var colNames = ['Id', 'Name', 'Url Slug', 'Description'];

    var columns = [];

    columns.push({
      name: 'Id',
      index: 'Id',
      //hidden: false,
      // sorttype: 'int',
      width: 50,
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
      sortable: true,
      editoptions: {
        size: 80,
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
        size: 80,
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
        rows: "5",
        cols: "100"
      }
    });

    $(gridName).jqGrid({
      url: '/admin/Tags',
      datatype: 'json',
      mtype: 'GET',
      height: 'auto',
      toppager: true,
      colNames: colNames,
      colModel: columns,
      pager: pagerName,
      rownumbers: true,
      rownumWidth: 40,
      rowNum: 17,
      rowList: [10, 20, 30, 50],
      sortname: 'Name',
        viewrecords: true, 
      jsonReader: {
        repeatitems: false
      }
    });

    var editOptions = {
      url: '/admin/EditTag',
      editCaption: 'Редактирование выбранного Тега',
      processData: "Сохранение...",
      closeAfterEdit: true,
      closeOnEscape: true,
      width: 700,
      afterSubmit: function (response, postdata) {
        var json = $.parseJSON(response.responseText);

        if (json) {
          $(gridName).jqGrid('setGridParam', { datatype: 'json' });
          return [json.success, json.message, json.id];
        }

        return [false, "Возникла ошибка при получении отвера с сервера", null];
      }
    };

    var addOptions = {
      url: '/admin/AddTag',
      addCaption: 'Создать новый тег',
      processData: "Сохранение...",
      closeAfterAdd: false,
      closeOnEscape: true,
      width: 700,
      afterSubmit: function (response, postdata) {
        var json = $.parseJSON(response.responseText);

        if (json) {
          $(gridName).jqGrid('setGridParam', { datatype: 'json' });
          return [json.success, json.message, json.id];
        }

          return [false, "Возникла ошибка при получении отвера с сервера", null];
      }
    };

    var deleteOptions = {
      url: '/admin/DeleteTag',
      caption: 'Удалить выбранный тег',
      processData: "Сохранение...",
      width: 500,
      msg: "Вы уверены что хотите удалить выбранный тег?",
      closeOnEscape: true,
      afterSubmit: JustBlog.GridManager.afterSubmitHandler
    };

    // конфигурирование навигационной панели 
    $(gridName).jqGrid('navGrid', pagerName, {
      cloneToTop: true,
        search: false,
        view: true
     }, editOptions, addOptions, deleteOptions);

  };

//*****************************************TAGS GRID END **********************************************


//*****************************************USERS GRID END *********************************************
    JustBlog.GridManager.usersGrid = function (gridName, pagerName) {
        var colNames = ['Id', 'UserName','Email', 'EmailConfirmed',
            'PasswordHash',
            'SecurityStamp',
            'PhoneNumber',
            'PhoneNumberConfirmed',
            'TwoFactorEnabled',
            'LockoutEndDateUtc',
            'LockoutEnabled',
            'AccessFailedCount'
         ];

        var columns = [];

        columns.push({
            name: 'Id',
            index: 'Id',
            width: 50,
            key: true,
            editable: false,
            editoptions: {
                readonly: true
            }
        });

        columns.push({
            name: 'UserName',
            index: 'UserName',
            width: 150,
            editable: true,
            edittype: 'text',
            sortable: true,
            editoptions: {
                size: 30,
                maxlength: 50
            },
            editrules: {
                required: true
            }
        });

        columns.push({
            name: 'Email',
            index: 'Email',
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
            name: 'EmailConfirmed',
            index: 'EmailConfirmed',
            width: 50,
            editable: true,
            edittype: 'text',
            sortable: false,
            editoptions: {
                size: 20,
                maxlength: 50
            },
            editrules: {
                required: true
            }
        });
        columns.push({
            name: 'PasswordHash',
            index: 'PasswordHash',
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
            name: 'SecurityStamp',
            index: 'SecurityStamp',
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
            name: 'PhoneNumber',
            index: 'PhoneNumber',
            width: 150,
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
            name: 'PhoneNumberConfirmed',
            index: 'PhoneNumberConfirmed',
            width: 50,
            editable: true,
            edittype: 'text',
            sortable: false,
            editoptions: {
                size: 20,
                maxlength: 50
            },
            editrules: {
                required: true
            }
        });

        columns.push({
            name: 'TwoFactorEnabled',
            index: 'TwoFactorEnabled',
            width: 50,
            editable: true,
            edittype: 'text',
            sortable: false,
            editoptions: {
                size: 20,
                maxlength: 50
            },
            editrules: {
                required: true
            }
        });

        columns.push({
            name: 'LockoutEndDateUtc',
            index: 'LockoutEndDateUtc',
            width: 50,
            editable: true,
            edittype: 'text',
            sortable: false,
            editoptions: {
                size: 20,
                maxlength: 50
            },
            editrules: {
                required: true
            }
        });


        columns.push({
            name: 'LockoutEnabled',
            index: 'LockoutEnabled',
            width: 50,
            editable: true,
            edittype: 'text',
            sortable: false,
            editoptions: {
                size: 20,
                maxlength: 50
            },
            editrules: {
                required: true
            }
        });

        columns.push({
            name: 'AccessFailedCount',
            index: 'AccessFailedCount',
            width: 50,
            editable: true,
            edittype: 'text',
            sortable: false,
            editoptions: {
                size: 20,
                maxlength: 50
            },
            editrules: {
                required: true
            }
        });
   

        $(gridName).jqGrid({
            url: '/admin/Users',
            datatype: 'json',
            mtype: 'GET',
            height: 'auto',
            toppager: true,
            colNames: colNames,
            colModel: columns,
            pager: pagerName,
            rownumbers: true,
            rownumWidth: 40,
            rowNum: 17,
            rowList: [10, 20, 30, 50],
            sortname: 'UserName',
            viewrecords: true,
            jsonReader: {
                repeatitems: false
            }
        });

        var editOptions = {};
        var addOptions = {};
        var deleteOptions = {
            url: '/admin/DeleteUser',
            caption: 'Удалить выбранного пользователя?',
            processData: "Сохранение...",
            width: 500,
            msg: "Вы уверены что хотите удалить выбранного пользователя?",
            closeOnEscape: true,
            afterSubmit: JustBlog.GridManager.afterSubmitHandler
        };

        // конфигурирование навигационной панели 
        $(gridName).jqGrid('navGrid', pagerName, {
            cloneToTop: true,
            search: false,
            add: false,
            edit:false,
            view: true
        }, editOptions, addOptions, deleteOptions);

    };

//*****************************************USERS GRID END *********************************************


//**************************************** CHATS GRID **********************************************
    JustBlog.GridManager.chatsGrid = function (gridName, pagerName) {
        var colNames = ['Id', 'Message', 'DateMessage', 'UserId','User.UserName'];

        var columns = [];

        columns.push({
            name: 'Id',
            index: 'Id',
            width: 50,
            key: true,
          });

        columns.push({
            name: 'Message',
            index: 'Message',
            width: 500,
         });

        columns.push({
            name: 'DateMessage',
            index: 'DateMessage',
            width: 200,
            datefmt: 'd/m/Y'
           });

        columns.push({
            name: 'UserId',
            index: 'UserId',
            width: 200,
        
        });
        columns.push({
            name: 'User.UserName',
            index: 'User',
            width: 200,

        });

        $(gridName).jqGrid({
            url: '/admin/Chats',
            datatype: 'json',
            mtype: 'GET',
            height: 'auto',
            toppager: true,
            colNames: colNames,
            colModel: columns,
            pager: pagerName,
            rownumbers: true,
            rownumWidth: 40,
            rowNum: 30,
            rowList: [30, 50,100,200],
            sortname: 'DateMessage',
            viewrecords: true,
            jsonReader: {
                repeatitems: false
            }
        });


        var editOptions = {};
        var addOptions = {};
        var deleteOptions = {
            url: '/admin/DeleteChat',
            caption: 'Удалить выбранное сообщение',
            processData: "Сохранение...",
            width: 500,
            msg: "Вы уверены что хотите удалить выбранное сообщение?",
            closeOnEscape: true,
            afterSubmit: JustBlog.GridManager.afterSubmitHandler
        };

        // конфигурирование навигационной панели 
        $(gridName).jqGrid('navGrid', pagerName, {
            cloneToTop: true,
            search: false,
            add: false,
            edit: false,
            view: true
        }, editOptions, addOptions, deleteOptions);


    };

//*****************************************CHATS GRID END **********************************************

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
          case 3:
            fn = gdMgr.usersGrid;
            gridName = "#tableUsers";
            pagerName = "#pagerUsers";
            break;
          case 4:
           fn = gdMgr.chatsGrid;
           gridName = "#tableChats";
           pagerName = "#pagerChats";
           break;
        };

        fn(gridName, pagerName);
        ui.tab.isLoaded = true;
        }
       
    }

  });


  
    //var a = JustBlog.GridManager.tagsGrid;
    
});




























