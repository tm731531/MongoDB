using MongoDB.Driver;
using MongoDB.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MongoDB.Controllers
{
    public class memberController : ApiController
    {
        // GET api/<controller>
        public GetMemberListResponse Get()
        {
            var response = new GetMemberListResponse();
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            MongoDatabaseBase db = client.GetDatabase("ntut") as MongoDatabaseBase;
            var memberCollection = db.GetCollection<MemberCollection>("members");
            var query = new BsonDocument();// Builders<MemberCollection>.Filter.Eq(e => e.uid, id);
            var cursor = memberCollection.Find(query).ToListAsync().Result;

            foreach (var doc in cursor)
            {
                response.list.Add(
                    new MemberInfo()
                    {
                        uid = doc.uid,
                        name = doc.name,
                        phone = doc.phone
                    }
                    );
            }
            return response;
        }

        // GET api/<controller>/5
        public GetMemberInfoResponse Get(string id)
        {
            var response = new GetMemberInfoResponse();
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            MongoDatabaseBase db = client.GetDatabase("ntut") as MongoDatabaseBase;
            var memberCollection = db.GetCollection<MemberCollection>("members");
            var query = Builders<MemberCollection>.Filter.Eq(e => e.uid, id);
            var doc = memberCollection.Find(query).ToListAsync().Result.FirstOrDefault();
            if (doc != null)
            {

               response.data.uid = doc.uid;
                response.data.name = doc.name;
                response.data.phone = doc.phone;
                
            }
            else
            {
                response.ok = false;
                response.errMsg = $"Number: is not exist";
            }
            return response;

        }

        // POST api/<controller>
        public AddMemberResponse Post(AddMemberRequest request)
        {
            var response = new AddMemberResponse();
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            MongoDatabaseBase db = client.GetDatabase("ntut") as MongoDatabaseBase;
            var memberCollection = db.GetCollection<MemberCollection>("members");
            var query = Builders<MemberCollection>.Filter.Eq(e => e.uid, request.uid);
            var doc = memberCollection.Find(query).ToListAsync().Result.FirstOrDefault();
            if (doc == null)
            {
                memberCollection.InsertOne(new MemberCollection()
                {
                    _id = ObjectId.GenerateNewId(),
                    uid = request.uid,
                    name = request.name,
                    phone = request.phone
                });
            }
            else
            {
                response.ok = false;
                response.errMsg = $"Number: {request.uid} is exist";
            }
            return response;

        }

        // PUT api/<controller>/5
        public EditMemberResponse Put(EditMemberRequest request)
        {
            var response = new EditMemberResponse();
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            MongoDatabaseBase db = client.GetDatabase("ntut") as MongoDatabaseBase;
            var memberCollection = db.GetCollection<MemberCollection>("members");
            var query = Builders<MemberCollection>.Filter.Eq(e => e.uid, request.uid);
            var doc = memberCollection.Find(query).ToListAsync().Result.FirstOrDefault();
            if (doc != null)
            {
                var update = Builders<MemberCollection>.Update.Set("name", request.name).Set("phone", request.phone);
                memberCollection.UpdateOne(query, update);
                //memberCollection.InsertOne(new MemberCollection()
                //{
                //    _id = ObjectId.GenerateNewId(),
                //    uid = request.uid,
                //    name = request.name,
                //    phone = request.phone
                //});
            }
            else
            {
                response.ok = false;
                response.errMsg = $"Number: {request.uid} is update fail";
            }
            return response;
        }

        // DELETE api/<controller>/5
        public DeleteMemberResponse Delete(string id)
        {
            var response = new DeleteMemberResponse();
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            MongoDatabaseBase db = client.GetDatabase("ntut") as MongoDatabaseBase;
            var memberCollection = db.GetCollection<MemberCollection>("members");
            var query = Builders<MemberCollection>.Filter.Eq(e => e.uid, id);


            var result = memberCollection.DeleteOne(query);
            if (result.DeletedCount != 0)
            {

            }
            else
            {
                response.ok = false;
                response.errMsg = $"Number: {id} Delete  fail";
            }
            return response;
        }
    }
}