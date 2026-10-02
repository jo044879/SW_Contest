# clone 하기
git clone https://github.com/jo044879/SW_Contest.git 

# 브런치 확인
git branch -a 
로 확인 자기 이름 브러니에서 코드 작성 예정

# 브런치로 들어가기 

git switch ### (###은 자기 영문 이름 넣기)

---

# 코드 작업 시작 전

git pull origin main 으로 병합된 버전을 가져와서 코드 작업을 시작해야함!

# 코드 작업 이후

git add .
git commit -m " {코드 작업한 내용 작성} "
git push origin {자기 코드 작업한 영문 이름}

## 중요! : main에다 넣을 경우에 바로 바뀌기 때문에 코드가 오류가 난 상태이면 전체에 영향을 줌 반드시 자기 branch에 넣기

# push 이후 

github 사이트에 들어가서 pull request로 보내기만 하기!

