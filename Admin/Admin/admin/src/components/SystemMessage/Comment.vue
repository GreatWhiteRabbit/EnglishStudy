<template>
  <div class="main">
    <el-row class="comment" >
      <el-row >
        <el-input
            type="textarea"
            placeholder="分享你的学习心得吧！！！"
            :autosize="{ minRows:3 }"
            style="width: 60%;font-size: 20px;"
            @input="getContentLength"
            v-model="inputContent"
        >
        </el-input>
      </el-row>
      <el-row style="margin-left: 50%">
        <span  style="font-size: 20px;color: #f56c6c">还可输入{{maxLength-contentLength}}字</span>
      </el-row>
      <el-row>
        <el-col :span="12">
          <el-col :span="3">
            <el-button @click="showCard(1)"  round>图片</el-button>
            <el-card v-show="showUpload" class="box-card">
              <div slot="header" class="clearfix">
                <el-button @click="showUpload = false" style="float: right; padding: 3px 0" type="text">X</el-button>
              </div>
              <el-upload
                  v-if="imageUrl === ''"
                  class="avatar-uploader"
                  :action=actionURL
                  list-type="picture-card"
                  :show-file-list="false"
                  :on-success="handleAvatarSuccess"
                  :before-upload="beforeAvatarUpload"
                  :onError="uploadError"
                  :headers="Header"
                  :name="file"
              >
                <i class="el-icon-plus"></i>
              </el-upload>
              <img height="60" v-else :src="imageUrl" class="avatar" />
            </el-card>
          </el-col>
          <el-col :span="3">
            <el-button @click="showCard(2)" round>表情</el-button>
            <el-card v-show="showEmotion" class="box-card">
              <div slot="header" class="clearfix">
                <span>表情</span>
                <el-button @click="showEmotion = false" style="float: right; padding: 3px 0" type="text">X</el-button>
              </div>
              <div class="emotion-box" style="height: 200px " >
                <el-scrollbar style="width: 100%;height: 100%;">
                  <div class="emotion-box-line"  >
                    <li class="emotion-item" v-for="(item,index) in emotion" :key="index"
                        @click="chooseEmotion(item,index)" >
                      <img style="height: 25px;width: 25px" :src=item.url />
                    </li>
                  </div>
                </el-scrollbar>
              </div>
            </el-card>
          </el-col>
          <el-col :span="3">
            <el-button @click="showCard(3)" round>链接</el-button>
            <el-card v-show="showLink" class="box-card">
              <div slot="header" class="clearfix">
                <span>链接</span>
                <el-button @click="showLink = false" style="float: right; padding: 3px 0" type="text">X</el-button>
              </div>
              <div>
                <el-input
                    placeholder="请输入网址"
                    v-model="link"
                />
                <el-input
                    placeholder="请输入名称"
                    v-model="linkName"
                />
              </div>
              <el-row style="margin-top: 4px;">
                <el-col :span="5">
                  <el-button @click="cancelLink" round>取消</el-button>
                </el-col>
                <el-col :span="3">
                  <el-button @click="addLink" type="primary" round>确定</el-button>
                </el-col>
              </el-row>
            </el-card>
          </el-col>
        </el-col>
        <el-col :span="12">
          <el-button
              type="primary"
              round
              style="font-size: 18px"
              @click="submitComment"
          >发布</el-button>
        </el-col>
      </el-row>
    </el-row>
    <el-row class="comment">
      <el-row v-show="total > 10">
        <el-pagination
            v-show="total > 10"
            background
            layout="sizes, prev, pager, next"
            :page-size="currentSize"
            :current-page.sync="currentPage"
            @current-change="handleCurrentChange"
            @size-change="handleSizeChange"
            page-sizes="10"
            :total="total"
            style="display: flex;justify-content: center;margin-bottom:10px;margin-right: 30%;margin-top: 50px;"
        >
        </el-pagination>
      </el-row>
      <comment-list :list="commentList"></comment-list>
      <el-row v-show="total > 10">
        <el-pagination
            v-show="total > 10"
            background
            layout="sizes, prev, pager, next"
            :page-size="currentSize"
            :current-page.sync="currentPage"
            @current-change="handleCurrentChange"
            @size-change="handleSizeChange"
            page-sizes="10"
            :total="total"
            style="display: flex;justify-content: center;margin-bottom:100px;margin-right: 30%;"
        >
        </el-pagination>
      </el-row>

    </el-row>

  </div>

</template>

<script>
import CommentList from "@/components/SystemMessage/CommentList";
export default {
  name:"Comment",
  components: {CommentList},
  data() {
    return {
      actionURL:'/apis/user/upload/comment',
      inputContent:'', // 内容
      contentLength: 0, // 内容长度
      maxLength:300, // 内容最大长度,
      commentList:[],// 评论列表
      total:0, // 评论总数,
      currentPage:1, // 当前页
      currentSize:10, // 当前页面大小
      emotion:[
        { title: "微笑", url: "https://localhost:7031/resouce/image/12f6a6f1-878f-4689-861b-b8d846995d38.gif" },
        { title: "嘻嘻", url: "https://localhost:7031/resouce/image/31c8036b-6fa8-488b-af61-eac792592443.gif" },
        { title: "哈哈", url: "https://localhost:7031/resouce/image/588089f5-0630-4d78-86b4-d00b28744c07.gif" },
        { title: "可爱", url: "https://localhost:7031/resouce/image/d01baa4d-4201-448f-91bb-6708bed6682f.gif" },
        { title: "可怜", url: "https://localhost:7031/resouce/image/716f5a4b-6505-4cf5-8b72-547364b031fb.gif" },
        { title: "挖鼻", url: "https://localhost:7031/resouce/image/447b67e4-b2d9-4851-99bd-ac53b77d56fe.gif" },
        { title: "吃惊", url: "https://localhost:7031/resouce/image/ebc9c41d-f14f-40e6-ace0-9f4a0a9ff337.gif" },
        { title: "害羞", url: "https://localhost:7031/resouce/image/ce25556b-1193-4e16-8ca6-54700c8a3d16.gif" },
        { title: "挤眼", url: "https://localhost:7031/resouce/image/a9bb82be-adb9-4884-b9db-60fe3d2fa6fe.gif" },
        { title: "闭嘴", url: "https://localhost:7031/resouce/image/45e92f32-3a3f-4824-96b9-40f8dfdc281d.gif" },
        { title: "鄙视", url: "https://localhost:7031/resouce/image/f4978e13-de00-4d4a-9a42-c7a75c6d914c.gif" },
        { title: "爱你", url: "https://localhost:7031/resouce/image/9cf356db-42ce-4212-ae5d-eec303619edd.gif" },
        { title: "泪", url: "https://localhost:7031/resouce/image/85990056-2523-4691-b678-c3d48836f5e4.gif" },
        { title: "偷笑", url: "https://localhost:7031/resouce/image/c9e52d7a-e49a-4914-aab1-b93bb6966aaf.gif" },
        { title: "亲亲", url: "https://localhost:7031/resouce/image/5ec021d7-9d23-4b04-8abc-59541412255b.gif" },
        { title: "生病", url: "https://localhost:7031/resouce/image/60b59cc1-0a62-49e7-8a59-87f0ea09b21c.gif" },
        { title: "太开心", url: "https://localhost:7031/resouce/image/b81b55c9-24d6-42ce-9b50-7affa5fa0fb7.gif" },
        { title: "白眼", url: "https://localhost:7031/resouce/image/694eee54-cf5c-4aa7-a206-e3e03e6f5fcf.gif" },
        { title: "右哼哼", url: "https://localhost:7031/resouce/image/07e3ffa2-2559-4bef-a5af-93a815eef43c.gif" },
        { title: "左哼哼", url: "https://localhost:7031/resouce/image/225192b1-2f1b-4b63-abc5-52559dde8d16.gif" },
        { title: "嘘", url: "https://localhost:7031/resouce/image/b1d4acef-15e2-4857-a530-ad1d11a97543.gif" },
        { title: "衰", url: "https://localhost:7031/resouce/image/12b3193b-7c7e-46e8-aae0-7a69ad9f97ae.gif" },
        { title: "吐", url: "https://localhost:7031/resouce/image/77711ac0-65dd-40f8-ad64-22cb3733e40f.gif" },
        { title: "哈欠", url: "https://localhost:7031/resouce/image/34562a4c-52c1-422e-9c5b-2dcf8c2a4eb0.gif" },
        { title: "抱抱", url: "https://localhost:7031/resouce/image/789330d7-cff1-4a86-bb4d-fc7ed1335592.gif" },
        { title: "怒", url: "https://localhost:7031/resouce/image/0d4ee6d7-521c-4477-bd58-b9be4825c83a.gif" },
        { title: "疑问", url: "https://localhost:7031/resouce/image/a0a0d9b5-bc81-46bf-a51e-95eb8e564fc6.gif" },
        { title: "馋嘴", url: "https://localhost:7031/resouce/image/c81ef61e-14a4-4272-8c3f-26a1f46b77c4.gif" },
        { title: "拜拜", url: "https://localhost:7031/resouce/image/5a3c470e-9211-44c2-8590-7e174ceceab4.gif" },
        { title: "思考", url: "https://localhost:7031/resouce/image/6950cd6c-dcc1-418f-860f-d44f15c3768f.gif" },
        { title: "汗", url: "https://localhost:7031/resouce/image/9244af58-23d1-42d6-b1c3-97b6fa2262e3.gif" },
        { title: "困", url: "https://localhost:7031/resouce/image/5d0fc131-166b-4481-9c99-e91ee2ebe31a.gif" },
        { title: "睡", url: "https://localhost:7031/resouce/image/7d858e27-7d0f-404c-87d8-60d617ad19c6.gif" },
        { title: "钱", url: "https://localhost:7031/resouce/image/8275b5a8-6025-4c22-b75c-c1e8fdd0aedd.gif" },
        { title: "失望", url: "https://localhost:7031/resouce/image/0e814383-6d44-4077-a107-6e037d28b0d8.gif" },
        { title: "酷", url: "https://localhost:7031/resouce/image/d9def7ae-d0f1-481f-a375-d02e4d518a5f.gif" },
        { title: "色", url: "https://localhost:7031/resouce/image/71d74f6e-d1d1-45f1-89b1-5ef2ea8dce72.gif" },
        { title: "哼", url: "https://localhost:7031/resouce/image/06612d5a-fcb3-41d2-aca1-1859bd38f96a.gif" },
        { title: "鼓掌", url: "https://localhost:7031/resouce/image/29ca220a-4632-436b-aafc-41afe12f3399.gif" },
        { title: "晕", url: "https://localhost:7031/resouce/image/fe2db77e-16da-4cb7-a9db-ddea97c89d44.gif" },
        { title: "悲伤", url: "https://localhost:7031/resouce/image/6450bbe2-4123-4505-b370-52b0d08f0588.gif" },
        { title: "抓狂", url: "https://localhost:7031/resouce/image/faaad713-75fd-4d9e-beda-78ebf4c3fd32.gif" },
        { title: "黑线", url: "https://localhost:7031/resouce/image/ca55f889-439e-428d-bfef-e7081085bdc3.gif" },
        { title: "阴险", url: "https://localhost:7031/resouce/image/8064bacf-f3cb-4d1a-8582-b725e1e00997.gif" },
        { title: "怒骂", url: "https://localhost:7031/resouce/image/c710cc1a-6dd1-4fc6-a354-aba105704336.gif" },
        { title: "互粉", url: "https://localhost:7031/resouce/image/b631bd0a-84b9-409c-8d3b-c8c77a8a1d13.gif" },
        { title: "书呆子", url: "https://localhost:7031/resouce/image/9a6cde17-d56d-408c-9972-9f81e3d83f16.gif" },
        { title: "愤怒", url: "https://localhost:7031/resouce/image/6f15f074-6f48-4ecc-872a-893b0b52cc8d.gif" },
        { title: "感冒", url: "https://localhost:7031/resouce/image/79da439b-2b0c-4ae7-ae56-7359b5861b67.gif" },
        { title: "心", url: "https://localhost:7031/resouce/image/d2d54296-a25b-47ac-87db-a6e80d76eae4.gif" },
        { title: "伤心", url: "https://localhost:7031/resouce/image/84495e67-984f-4727-8b08-e71418e32f08.gif" },
        { title: "猪", url: "https://localhost:7031/resouce/image/dfc92659-f358-44bc-a8e7-8d4980172230.gif" },
        { title: "熊猫", url: "https://localhost:7031/resouce/image/199d9ab5-11bb-4d7f-a5a4-26f6f26c57e7.gif" },
        { title: "兔子", url: "https://localhost:7031/resouce/image/e59e7fcf-f782-49a3-87f1-9bcab2d53fc5.gif" },
        { title: "喔克", url: "https://localhost:7031/resouce/image/0156a676-8c57-48a7-b77a-86994803aefd.gif" },
        { title: "耶", url: "https://localhost:7031/resouce/image/3d46e4e4-456c-408d-9072-828e43a1aa43.gif" },
        { title: "棒棒", url: "https://localhost:7031/resouce/image/77bddff4-cc81-4351-bf1c-2b629ed3e433.gif" },
        { title: "不", url: "https://localhost:7031/resouce/image/7cb0b371-22e1-405e-8279-0cb4a302c1f1.gif" },
        { title: "赞", url: "https://localhost:7031/resouce/image/bd802a85-c877-489b-a2b1-015840eded26.gif" },
        { title: "来", url: "https://localhost:7031/resouce/image/0b72872f-9068-438f-9811-c07ae822c95c.gif" },
        { title: "弱", url: "https://localhost:7031/resouce/image/c09ec184-82e9-4f16-b54e-5079ae359ff0.gif" },
        { title: "草泥马", url: "https://localhost:7031/resouce/image/5ef46f7e-9e35-48a0-805a-955c42f2975d.gif" },
        { title: "神马", url: "https://localhost:7031/resouce/image/88b772b2-97f8-44c9-9d78-6087b678bcb6.gif" },
        { title: "囧", url: "https://localhost:7031/resouce/image/69c1217a-7d2a-4214-97cb-5e26b549d32e.gif" },
        { title: "浮云", url: "https://localhost:7031/resouce/image/71111f4e-932d-483b-b2a2-8b94c9e72118.gif" },
        { title: "给力", url: "https://localhost:7031/resouce/image/cd07b452-c7b2-4261-a815-5513893e1adc.gif" },
        { title: "围观", url: "https://localhost:7031/resouce/image/c621823e-c7f3-4f03-88d4-fd2931ca410b.gif" },
        { title: "威武", url: "https://localhost:7031/resouce/image/995db947-c6cb-43e6-a7e4-9e91ddbf8055.gif" },
        { title: "话筒", url: "https://localhost:7031/resouce/image/832a3240-f099-4edc-97d9-6c4c2cfa1f5c.gif" },
        { title: "蜡烛", url: "https://localhost:7031/resouce/image/03099ee8-7fa7-481e-93ce-703f3993ba27.gif" },
        { title: "蛋糕", url: "https://localhost:7031/resouce/image/1bf5df5c-42ed-45e3-ba9d-75a26aa8b8dc.gif" },
        { title: "发红包", url: "https://localhost:7031/resouce/image/3385eece-ea09-432b-837e-8955f6ce1415.gif" }, { title: "抱拳", url: "https://localhost:7031/resouce/image/d31f18e1-a06d-40f0-9872-cb0d836f331d.png" },

      ],
      showEmotion:false,// 显示表情包框
      showUpload:false,// 显示上传
      showLink:false,// 显示链接
      link:'',// 链接
      linkName:'',// 链接名称
      Header:{
        Authorization: "Bearer" + " " +  localStorage.getItem("token")
      },
      imageUrl:'',

    }
  },
  methods:{
    // 发布评论
    submitComment() {
      let submit = true;
      // 首先判断是否登录
      if(localStorage.getItem("userId") === null && localStorage.getItem("token") === null) {
        submit = false;
        this.$message.error("请先登录");
        this.$router.push('/login');
        return;
      }
      // 判断字数是否超标
      if(this.contentLength === 0) {
        submit = false;
        this.$message.error("评论内容不能为空");
        return;
      }
      if(this.contentLength > this.maxLength) {
        submit = false;
        this.$message.error("字数超出范围了");
        return;
      }
      // 提交评论，这里提交的评论是一级评论
      if(submit === true) {
        let userId = localStorage.getItem("userId");
        this.HttpPost('/comment/add',{
          userId:userId,
          content:this.inputContent
        })
            .then(res => {
              if(res.data === 0) {
                this.$message.error("发布评论失败");
              }
              else if(res.data === -1) {
                this.$message.error("字数超出限制");
              }
              else {
                this.$message.success("发布成功");
                this.inputContent = "";
                this.getCommentList();
              }
            })
      }
    },
    // 计算输入的字符，包括表情包，链接等
    getContentLength() {
      // inputContent的长度需要重新计算
      this.contentLength = this.inputContent.length;
    },

    // 选择表情包
    chooseEmotion(item,index) {
      this.inputContent += "$" + this.emotion[index].title + ":";
      this.getContentLength();
    },
    // 添加链接
    addLink() {
      this.showLink = false;
      if(this.link !== "" && this.linkName !== "") {
        this.inputContent += "#" + this.link + "***" + this.linkName + ";";
      }
      this.getContentLength();
      this.link = "";
      this.linkName = "";
    },
    // 取消添加链接
    cancelLink() {
      this.showLink = false;
      this.link = "";
      this.linkName = "";
    },
    // 添加图片
    addImage() {
      this.inputContent += "~" + this.imageUrl + "}";
      this.getContentLength();
    },

    handleAvatarSuccess(res, file) {
      this.imageUrl= res.data;
      this.$notify({
        message:"上传成功",
        type:'success',
        title:'系统提示'
      });
      this.addImage();
    },
    beforeAvatarUpload(file) {
      const arr = [
        "image/jpeg",
        "image/gif",
        "image/jpg",
        "image/png",
        "image/x-png",
        "image/pjpeg",
      ];
      const isJPG = arr.indexOf(file.type) >= 0;
      const isLt2M = file.size / 1024 / 1024 < 4;
      if (!isJPG) {
        this.$notify({
          message:'文件格式不正确',
          title:"系统提示",
          type:'error'
        })
      }
      if (!isLt2M) {
        this.$notify({
          message:'图片大小不能超过4MB',
          title:"系统提示",
          type:'error'
        })
      }
      return isJPG && isLt2M;
    },
    uploadError() {
      this.$notify({
        message:"上传失败",
        title:'系统提示',
        type:"error"
      })
    },

    // 显示表情框或者图片框或者链接框
    showCard(index) {
      if(index === 2) {
        this.showEmotion = !this.showEmotion;
        this.showLink = false;
        this.showUpload = false;
      }
      else if(index === 3) {
        this.showEmotion = false;
        this.showLink = !this.showLink;
        this.showUpload = false;
      }
      else if(index === 1) {
        this.showEmotion = false;
        this.showLink = false;
        this.showUpload = !this.showUpload;
        this.imageUrl = "";
      }
    },
    // 获取评论的数据
    getCommentList() {
      this.HttpGet('/comment/first/' + this.currentPage + '/' + this.currentSize)
          .then(res =>  {
            this.total = res.data.total;
            this.commentList = res.data.firstLeverCommentDTOList;
          })
    },
    // 调整页面尺寸
    handleSizeChange(val) {
      this.currentSize = val;
      // 重新获取数据
      this.getCommentList();

    },
    // 调整页面大小
    handleCurrentChange(val) {
      this.currentPage = val;
      this.getCommentList();
    },
  },
  created() {
    this.getCommentList();
  }
};
</script>

<style lang="stylus" scoped>
.main{
  background-color white;
  height 100%;
}
.comment {
  margin-left 2%;
}

.text {
  font-size: 14px;
}
.clearfix:before,
.clearfix:after {
  display: table;
  content: "";
}
.clearfix:after {
  clear: both
}

.box-card {
  width: 300px;
}
.emotion-box {
  margin: 0 auto;
  width: 100%;
  box-sizing: border-box;
  padding: 5px;
  border: 1px solid #b4b4b4;
  overflow: hidden;
  overflow-y: auto;
}
.emotion-box-line {
  display: flex;
  flex-wrap wrap;
}
.emotion-item {
  flex: 1;
  text-align: center;
  cursor: pointer;
}
.avatar-img {
  top: 0;
  right: 150px;
}
.avatar {
  height: 148px;
  max-width: 148px;
  border-radius: 5px;
  display: block;
  margin-top 40px;
}
</style>
