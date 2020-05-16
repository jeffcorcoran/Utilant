var Album = function ()
{
    let me = this;

    me._albumTemplate = '';
    me._userDialogTemplate = '';
    me._albumPhotosTemplate = '';
    me._userPostDialogTemplate = '';

    me._activeAlbum = null;

    me._searchWaiter = null;
    me._searchResults = [];
    me._preSearchAlbum = {};

    me.Init = function ()
    {
        me._BuildPager();
        me._PullAlbum(1);
        me._AttachSearch();
        $('#dialog').dialog({ autoOpen: false });
    }

    me._AttachSearch = function ()
    {
        $('.searchBox').on('keyup', function (e)
        {
            me._SetSearchWait();
        });
    }

    me._SetSearchWait = function ()
    {
        window.clearTimeout(me._searchWaiter);

        if (Object.keys(me._GetSearchValues()).length == 0)
        {
            me._searchResults = [];
            me._activeAlbum = me._preSearchAlbum;
            me._SetAlbumInView();
            return;
        }

        me._searchWaiter = window.setTimeout(function ()
        {
            me._PerformSearch();
        }, 800);
    }

    me._GetSearchValues = function ()
    {
        let vals = {};

        $.each($('.searchBox'), function (idx, el)
        {
            if ($(el).val() != '')
            {
                vals[$(el).data('param')] = $(el).val();
            }
        });

        return vals;
    };

    me._PerformSearch = function ()
    {
        window.clearTimeout(me._searchWaiter);

        me._GetSearch(me._GetSearchValues());
    }

    me._GetSearch = function (vals)
    {
        if (vals.length == 0)
        {
            me._DisplayPagedAlbums();
            return;
        }

        $.ajax({
            type: "GET",
            url: '/album/search?' + $.param(vals),
            dataType: "json",
            contentType: "application/json",
            success: function (result)
            {
                me._preSearchAlbum = me._activeAlbum;
                me._searchResults = result;
                if (me._searchResults.length > 0)
                {
                    me._PullAlbum(me._searchResults[0]);
                }
                else
                {
                    me._activeAlbum = null;
                    me._DisplayPagedAlbums();
                }

            },
            error: function (xhr, textStatus, errorThrown)
            {

            }
        });
    }

    me._BuildPager = function ()
    {
        $('<div id="previousAlbum">&lt;</div>').on('click', function () { me._SetPreviousAlbumInView(); }).insertBefore($('#main'));
        $('<div id="nextAlbum">&gt;</div>').on('click', function () { me._SetNextAlbumInView(); }).insertAfter($('#main'));
    }

    me._PullAlbum = function (id)
    {
        $('body').addClass('loadSkrim');
        $.ajax({
            type: "GET",
            url: '/album/get/' + id,
            dataType: "json",
            contentType: "application/json",
            success: function (result)
            {
                me._activeAlbum = result;
                me._DisplayPagedAlbums();
                $('body').removeClass('loadSkrim');
            },
            error: function (xhr, textStatus, errorThrown)
            {

            }
        });
    }

    me._DisplayPagedAlbums = function ()
    {
        if (me._albumTemplate == '')
        {
            me._GetAlbumTemplate(me._DisplayPagedAlbums);
            return;
        }

        if (me._activeAlbum)
        {
            me._SetAlbumInView();
        }
        else
        {
            $('#main').html('<h2>No Albums Found</h2>')
        }
    }

    me._GetAlbumTemplate = function (callback)
    {
        $.get('/Content/htmlTemplates/AlbumPager.html', function (resp)
        {
            me._albumTemplate = Handlebars.compile(resp);

            if (typeof callback === "function")
            {
                callback();
            }
        });
    };

    me._SetPreviousAlbumInView = function ()
    {
        if (me._searchResults.length == 0)
        {
            me._PullAlbum(me._activeAlbum.id - 1);
        }
        else
        {
            let idx = me._searchResults.indexOf(me._activeAlbum.id);
            idx--;
            idx = (idx < 0) ? me._searchResults.length - 1 : idx;
            console.log(idx);
            me._PullAlbum(me._searchResults[idx]);
        }
    }

    me._SetNextAlbumInView = function ()
    {
        if (me._searchResults.length == 0)
        {
            me._PullAlbum(me._activeAlbum.id + 1);
        }
        else
        {
            let idx = me._searchResults.indexOf(me._activeAlbum.id);
            idx++;
            idx = (idx > me._searchResults.length - 1) ? 0 : idx;
            me._PullAlbum(me._searchResults[idx]);
        }
    }

    me._SetAlbumInView = function ()
    {
        $('#albumPhotos').slideUp();
        $('#albumPhotos').empty();

        $('#main').html(me._albumTemplate(me._activeAlbum));
        $('#username').off('click').on('click', function () { me.ShowUserDetails(me._activeAlbum.AlbumOwner); });
        $('#albumTitle').off('click').on('click', function () { me.ShowAlbumPhotos(me._activeAlbum.Photos); });
        $('#posts').off('click').on('click', function () { me.ShowUserPosts(me._activeAlbum); });
    };

    me.ShowUserPosts = function ()
    {
        if (me._userPostDialogTemplate == '')
        {
            me.GetUserPostDialogTemplate(function () { me.ShowUserPosts(); });
            return;
        }
        $('#dialog').html(me._userPostDialogTemplate(me._activeAlbum.AlbumOwner));
        $('#postList').accordion({ heightStyle: 'content' });
        $('#dialog').dialog({ title: 'User Posts', width: 800, modal: true, maxHeight: 500 });
        $('#dialog').dialog('open');
    };

    me.GetUserPostDialogTemplate = function (callback)
    {
        $.get('/Content/htmlTemplates/UserPosts.html', function (resp)
        {
            me._userPostDialogTemplate = Handlebars.compile(resp);

            if (typeof callback === "function")
            {
                callback();
            }
        });
    };

    me.ShowUserDetails = function ()
    {
        if (me._userDialogTemplate == '')
        {
            me.GetUserDialogTemplate(function () { me.ShowUserDetails(); });
            return;
        }
        $('#dialog').html(me._userDialogTemplate(me._activeAlbum.AlbumOwner));
        $('#dialog').dialog({ title: 'User Details', width: 300, modal: true });
        $('#dialog').dialog('open');
    };

    me.GetUserDialogTemplate = function (callback)
    {
        $.get('/Content/htmlTemplates/UserDetails.html', function (resp)
        {
            me._userDialogTemplate = Handlebars.compile(resp);

            if (typeof callback === "function")
            {
                callback();
            }
        });
    };

    me.ShowAlbumPhotos = function ()
    {
        if (me._albumPhotosTemplate == '')
        {
            me._GetAlbumPhotosTemplate(function () { me.ShowAlbumPhotos(); });
            return;
        }
        $('#albumPhotos').html(me._albumPhotosTemplate(me._activeAlbum));
        $('#albumPhotos').slideDown();
    }

    me._GetAlbumPhotosTemplate = function (callback)
    {
        $.get('/Content/htmlTemplates/AlbumPhotos.html', function (resp)
        {
            me._albumPhotosTemplate = Handlebars.compile(resp);

            if (typeof callback === "function")
            {
                callback();
            }
        });
    };
};